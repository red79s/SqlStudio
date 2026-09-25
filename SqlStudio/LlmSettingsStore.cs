using System;
using CfgDataStore;
using LlmService;

namespace SqlStudio
{
    /// <summary>
    /// Reads and writes the LLM configuration in the application config store, so the settings
    /// dialog and the composition root agree on keys, defaults and provider parsing.
    /// </summary>
    internal static class LlmSettingsStore
    {
        private const string ProviderKey = "llm_provider";

        /// <summary>Kept for backwards compatibility with configs written before providers existed.</summary>
        private const string ApiKeyKey = "gemini_api_key";

        private const string ModelKey = "llm_model";
        private const string EndpointKey = "llm_endpoint";

        /// <summary>
        /// Loads the stored options. Model/endpoint are only assigned when present, so unset values
        /// fall back to the selected provider's defaults.
        /// </summary>
        public static LlmOptions Load(IConfigDataStore cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException(nameof(cfg));

            var options = new LlmOptions { Provider = ParseProvider(cfg.GetStringValue(ProviderKey)) };

            var apiKey = cfg.GetStringValue(ApiKeyKey);
            if (!string.IsNullOrWhiteSpace(apiKey))
                options.ApiKey = apiKey.Trim();

            var model = cfg.GetStringValue(ModelKey);
            if (!string.IsNullOrWhiteSpace(model))
                options.Model = model.Trim();

            var endpoint = cfg.GetStringValue(EndpointKey);
            if (!string.IsNullOrWhiteSpace(endpoint))
                options.Endpoint = endpoint.Trim();

            return options;
        }

        /// <summary>Persists the options (does not call <see cref="IConfigDataStore.Save"/>).</summary>
        public static void Store(IConfigDataStore cfg, LlmOptions options)
        {
            if (cfg == null)
                throw new ArgumentNullException(nameof(cfg));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            cfg.SetValue(ProviderKey, options.Provider.ToString());
            cfg.SetValue(ApiKeyKey, options.ApiKey);
            cfg.SetValue(ModelKey, options.Model);
            cfg.SetValue(EndpointKey, LlmProviderDefaults.NormalizeEndpoint(options.Provider, options.Endpoint));
        }

        /// <summary>
        /// True when the options are complete enough to start the service. Ollama serves local
        /// models without authentication, so it needs no API key.
        /// </summary>
        public static bool IsConfigured(LlmOptions options)
        {
            if (options == null)
                return false;

            return !LlmProviderDefaults.RequiresApiKey(options.Provider)
                || !string.IsNullOrWhiteSpace(options.ApiKey);
        }

        private static LlmProvider ParseProvider(string value)
        {
            LlmProvider provider;
            return Enum.TryParse(value, ignoreCase: true, out provider)
                ? provider
                : LlmProvider.OpenAiCompatible;
        }
    }
}
