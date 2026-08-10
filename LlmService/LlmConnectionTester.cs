using System.ClientModel;
using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace LlmService
{
    /// <summary>
    /// Verifies that an <see cref="LlmOptions"/> configuration actually works, by sending a minimal
    /// chat request to the configured endpoint. This exercises everything a real request needs —
    /// endpoint reachability, authentication and the model name — so a misconfiguration surfaces in
    /// the settings dialog instead of at the first SQL generation.
    /// </summary>
    public static class LlmConnectionTester
    {
        /// <summary>Cheapest prompt that still produces a completion.</summary>
        private const string ProbePrompt = "Reply with the single word: OK";

        /// <summary>How long to wait before giving up on an unresponsive endpoint.</summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Sends a probe request using <paramref name="options"/>. Never throws for configuration or
        /// transport problems — those come back as a failed <see cref="LlmConnectionTestResult"/> so
        /// callers can show the message directly.
        /// </summary>
        public static async Task<LlmConnectionTestResult> TestAsync(
            LlmOptions options,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(options);

            var effectiveTimeout = timeout ?? DefaultTimeout;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutSource.CancelAfter(effectiveTimeout);

                using var chatClient = ServiceCollectionExtensions.CreateChatClient(options);

                var messages = new List<ChatMessage> { new ChatMessage(ChatRole.User, ProbePrompt) };
                var response = await chatClient.GetResponseAsync(
                    messages, cancellationToken: timeoutSource.Token);

                stopwatch.Stop();

                // A response of any shape means endpoint, key and model all checked out. The text
                // itself is irrelevant — reasoning models may return nothing but thinking tokens.
                return LlmConnectionTestResult.Ok(
                    $"Connected to {options.Model} in {stopwatch.ElapsedMilliseconds} ms.",
                    response.Text ?? string.Empty);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return LlmConnectionTestResult.Failed(
                    $"The endpoint did not respond within {effectiveTimeout.TotalSeconds:0} seconds.");
            }
            catch (Exception ex)
            {
                return LlmConnectionTestResult.Failed(Describe(ex));
            }
        }

        /// <summary>
        /// Builds a message worth showing a user. HTTP failures carry the provider's own error text
        /// in the response body — without it a bare "Status: 404" says nothing about whether the
        /// endpoint, the model or the API key is at fault. Transport failures instead wrap the real
        /// cause several layers deep ("No such host is known", "connection refused").
        /// </summary>
        private static string Describe(Exception ex)
        {
            if (ex is ClientResultException clientError)
            {
                var body = TryReadResponseBody(clientError);
                return string.IsNullOrWhiteSpace(body)
                    ? $"HTTP {clientError.Status}: {clientError.Message}"
                    : $"HTTP {clientError.Status}:{Environment.NewLine}{body}";
            }

            var innermost = ex;
            while (innermost.InnerException != null)
            {
                innermost = innermost.InnerException;
            }

            return ReferenceEquals(innermost, ex) || innermost.Message == ex.Message
                ? ex.Message
                : $"{ex.Message} ({innermost.Message})";
        }

        /// <summary>
        /// Reads the error body off a failed request. The response may already be disposed or have
        /// no content, in which case the caller falls back to the exception message.
        /// </summary>
        private static string TryReadResponseBody(ClientResultException error)
        {
            try
            {
                var content = error.GetRawResponse()?.Content?.ToString();
                return string.IsNullOrWhiteSpace(content) ? string.Empty : content.Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    /// <summary>Outcome of an <see cref="LlmConnectionTester.TestAsync"/> probe.</summary>
    public sealed class LlmConnectionTestResult
    {
        private LlmConnectionTestResult(bool success, string message, string responseText)
        {
            Success = success;
            Message = message;
            ResponseText = responseText;
        }

        /// <summary>True when the configured endpoint answered the probe request.</summary>
        public bool Success { get; }

        /// <summary>Human-readable summary, suitable for display without further formatting.</summary>
        public string Message { get; }

        /// <summary>The model's reply, when it produced one. Empty on failure.</summary>
        public string ResponseText { get; }

        public static LlmConnectionTestResult Ok(string message, string responseText) =>
            new LlmConnectionTestResult(true, message, responseText);

        public static LlmConnectionTestResult Failed(string message) =>
            new LlmConnectionTestResult(false, message, string.Empty);
    }
}
