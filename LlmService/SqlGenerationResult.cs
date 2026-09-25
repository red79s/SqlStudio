namespace LlmService
{
    /// <summary>
    /// Result of a text-to-SQL generation request.
    /// </summary>
    public class SqlGenerationResult
    {
        /// <summary>The generated SQL query, with any markdown code fences removed.</summary>
        public string Sql { get; set; } = string.Empty;

        /// <summary>The raw, unprocessed model response. Useful for diagnostics.</summary>
        public string? RawResponse { get; set; }
    }
}
