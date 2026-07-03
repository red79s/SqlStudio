namespace LlmService
{
    /// <summary>
    /// Configuration for the LLM chat client. Endpoint/model are configurable so a different
    /// Gemini model — or another OpenAI-compatible provider — can be used without code changes.
    /// </summary>
    public class LlmOptions
    {
        /// <summary>API key for the LLM provider (e.g. a Google AI Studio Gemini key).</summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>Model name to use for chat completions.</summary>
        public string Model { get; set; } = "gemini-2.5-flash";

        /// <summary>OpenAI-compatible base endpoint. Defaults to Gemini's compatibility endpoint.</summary>
        public string Endpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta/openai/";
    }
}
