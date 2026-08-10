using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace LlmService
{
    /// <summary>
    /// Dependency-injection registration for <see cref="ILlmService"/>. This is the only
    /// provider-specific code — a different OpenAI-compatible provider (Gemini, a real OpenAI/Azure
    /// endpoint) or a local Ollama server is selected purely through <see cref="LlmOptions"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Placeholder credential for endpoints that don't authenticate. Ollama ignores the
        /// Authorization header, but the OpenAI client requires a non-empty key.
        /// </summary>
        private const string NoAuthApiKey = "ollama";

        /// <summary>
        /// Registers an <see cref="IChatClient"/> backed by the configured provider (an
        /// OpenAI-compatible endpoint such as Gemini, or an Ollama server), plus the
        /// <see cref="ILlmService"/> that uses it.
        /// </summary>
        public static IServiceCollection AddLlmService(this IServiceCollection services, LlmOptions options)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(options);

            services.AddChatClient(CreateChatClient(options));
            services.AddSingleton<ILlmService, LlmService>();

            return services;
        }

        /// <summary>
        /// Builds the provider-specific <see cref="IChatClient"/>. Both providers speak the
        /// OpenAI chat-completions protocol; they differ in endpoint and authentication.
        /// </summary>
        public static IChatClient CreateChatClient(LlmOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var apiKey = options.Provider == LlmProvider.Ollama && string.IsNullOrWhiteSpace(options.ApiKey)
                ? NoAuthApiKey
                : options.ApiKey;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    $"An API key is required for provider '{options.Provider}'.");
            }

            var endpoint = LlmProviderDefaults.NormalizeEndpoint(options.Provider, options.Endpoint);
            var clientOptions = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };

            return new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions)
                .GetChatClient(options.Model)
                .AsIChatClient();
        }
    }
}
