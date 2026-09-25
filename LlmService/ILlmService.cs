using Common;

namespace LlmService
{
    /// <summary>
    /// Generates SQL from natural-language input using an LLM.
    /// </summary>
    public interface ILlmService
    {
        /// <summary>
        /// Generates a SQL query for the given natural-language <paramref name="request"/>.
        /// When <paramref name="schema"/> is provided, the database's tables, columns and foreign
        /// keys are included as context so the generated SQL matches the actual database.
        /// </summary>
        /// <param name="request">The natural-language description of the desired query.</param>
        /// <param name="schema">Optional database schema context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SqlGenerationResult> GenerateSqlAsync(
            string request,
            IDatabaseSchemaInfo? schema = null,
            CancellationToken cancellationToken = default);
    }
}
