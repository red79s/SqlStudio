namespace LlmService
{
    /// <summary>
    /// Supported LLM back-ends.
    /// </summary>
    public enum LlmProvider
    {
        /// <summary>Any OpenAI-compatible cloud endpoint (Gemini, OpenAI, Azure, ...). Requires an API key.</summary>
        OpenAiCompatible = 0,

        /// <summary>A local (or self-hosted) Ollama server. No API key required.</summary>
        Ollama = 1
    }

    /// <summary>
    /// Configuration for the LLM chat client. Provider/endpoint/model are configurable so a different
    /// Gemini model — another OpenAI-compatible provider, or a local Ollama server — can be used
    /// without code changes. <see cref="Model"/> and <see cref="Endpoint"/> fall back to the defaults
    /// of the selected <see cref="Provider"/> when left unset.
    /// </summary>
    public class LlmOptions
    {
        private string? _model;
        private string? _endpoint;

        /// <summary>Which back-end to talk to. Defaults to an OpenAI-compatible cloud endpoint.</summary>
        public LlmProvider Provider { get; set; } = LlmProvider.OpenAiCompatible;

        /// <summary>
        /// API key for the LLM provider (e.g. a Google AI Studio Gemini key). Not used by
        /// <see cref="LlmProvider.Ollama"/>.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>Model name to use for chat completions. Empty means "provider default".</summary>
        public string Model
        {
            get => string.IsNullOrWhiteSpace(_model) ? LlmProviderDefaults.Model(Provider) : _model;
            set => _model = value;
        }

        /// <summary>OpenAI-compatible base endpoint. Empty means "provider default".</summary>
        public string Endpoint
        {
            get => string.IsNullOrWhiteSpace(_endpoint) ? LlmProviderDefaults.Endpoint(Provider) : _endpoint;
            set => _endpoint = value;
        }
    }

    /// <summary>
    /// Per-provider defaults and capability flags, so UI and composition code don't hard-code
    /// provider knowledge.
    /// </summary>
    public static class LlmProviderDefaults
    {
        /// <summary>Gemini's OpenAI-compatibility endpoint.</summary>
        public const string GeminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";

        /// <summary>Default local Ollama OpenAI-compatibility endpoint.</summary>
        public const string OllamaEndpoint = "http://localhost:11434/v1/";

        /// <summary>Default base endpoint for <paramref name="provider"/>.</summary>
        public static string Endpoint(LlmProvider provider) => provider switch
        {
            LlmProvider.Ollama => OllamaEndpoint,
            _ => GeminiEndpoint
        };

        /// <summary>Default model name for <paramref name="provider"/>.</summary>
        public static string Model(LlmProvider provider) => provider switch
        {
            LlmProvider.Ollama => "llama3.1",
            _ => "gemini-2.5-flash"
        };

        /// <summary>
        /// Gemini text/chat models offered in the settings UI, newest generation first. Image,
        /// audio/TTS, video, embedding and Live-API models are deliberately left out — they can't
        /// serve chat completions. So is gemini-2.5-flash-lite: Google has closed it to new users
        /// and it answers with 404. The list is a snapshot of what Google publishes; typing a model
        /// name that isn't here still works.
        /// </summary>
        private static readonly string[] GeminiModels =
        {
            "gemini-3.6-flash",
            "gemini-3.5-flash",
            "gemini-3.5-flash-lite",
            "gemini-3.1-pro-preview",
            "gemini-3.1-flash-lite",
            "gemini-3-flash-preview",
            "gemini-2.5-pro",
            "gemini-2.5-flash"
        };

        private static readonly string[] OllamaModels =
        {
            "llama3.1"
        };

        /// <summary>
        /// Models suggested for <paramref name="provider"/> in the settings UI. The list is a
        /// convenience only — any model name the provider accepts can still be typed by hand, so
        /// new models don't require a code change. Always contains <see cref="Model(LlmProvider)"/>.
        /// </summary>
        public static IReadOnlyList<string> SuggestedModels(LlmProvider provider) => provider switch
        {
            LlmProvider.Ollama => OllamaModels,
            _ => GeminiModels
        };

        /// <summary>
        /// True when the provider needs an API key before the service can be used. Ollama serves
        /// local models without authentication.
        /// </summary>
        public static bool RequiresApiKey(LlmProvider provider) => provider != LlmProvider.Ollama;

        /// <summary>
        /// Normalizes a user-supplied base endpoint. Ollama users typically know their server as
        /// <c>http://localhost:11434</c>, but the OpenAI-compatible surface lives under <c>/v1</c>,
        /// so the suffix is appended when missing.
        /// </summary>
        public static string NormalizeEndpoint(LlmProvider provider, string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                return Endpoint(provider);
            }

            var normalized = endpoint.Trim();

            if (provider == LlmProvider.Ollama)
            {
                var withoutSlash = normalized.TrimEnd('/');
                if (!withoutSlash.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
                {
                    normalized = withoutSlash + "/v1/";
                }
            }

            return normalized.EndsWith('/') ? normalized : normalized + "/";
        }
    }
}
