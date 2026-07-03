using Common;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace LlmService
{
    /// <summary>
    /// Default <see cref="ILlmService"/> implementation. Uses an injected
    /// <see cref="IChatClient"/> so the concrete provider (Gemini today, others later) and
    /// transport are decided at composition time, and the service stays fully unit-testable.
    /// </summary>
    public class LlmService : ILlmService
    {
        private const string SystemPrompt =
            "You are an expert SQL assistant. Given a natural-language request, and optionally a " +
            "description of the database schema, produce a single valid SQL query that fulfills the " +
            "request. Use only tables and columns that exist in the provided schema. Respond with " +
            "the SQL query only — no explanations, comments, or markdown code fences.";

        private readonly IChatClient _chatClient;
        private readonly ILogger<LlmService> _logger;

        public LlmService(IChatClient chatClient, ILogger<LlmService> logger)
        {
            _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SqlGenerationResult> GenerateSqlAsync(
            string request,
            IDatabaseSchemaInfo? schema = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request))
            {
                throw new ArgumentException("Request must not be empty.", nameof(request));
            }

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, SystemPrompt)
            };

            var schemaContext = SchemaPromptBuilder.Build(schema);
            var userContent = string.IsNullOrEmpty(schemaContext)
                ? request
                : $"Database schema:\n{schemaContext}\nRequest: {request}";
            messages.Add(new ChatMessage(ChatRole.User, userContent));

            _logger.LogDebug("Requesting SQL generation for prompt: {Request}", request);

            var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);

            var rawText = response.Text ?? string.Empty;
            var sql = CleanSql(rawText);

            _logger.LogDebug("Generated SQL: {Sql}", sql);

            return new SqlGenerationResult
            {
                Sql = sql,
                RawResponse = rawText
            };
        }

        /// <summary>
        /// Removes surrounding markdown code fences (``` or ```sql) that models sometimes emit,
        /// and trims surrounding whitespace.
        /// </summary>
        internal static string CleanSql(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var trimmed = text.Trim();

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                // Drop the opening fence line (``` or ```sql) ...
                var firstNewLine = trimmed.IndexOf('\n');
                if (firstNewLine >= 0)
                {
                    trimmed = trimmed.Substring(firstNewLine + 1);
                }

                // ... and the trailing closing fence.
                var closingFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
                if (closingFence >= 0)
                {
                    trimmed = trimmed.Substring(0, closingFence);
                }
            }

            return trimmed.Trim();
        }
    }
}
