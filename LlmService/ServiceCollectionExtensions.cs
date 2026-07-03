using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace LlmService
{
    /// <summary>
    /// Dependency-injection registration for <see cref="ILlmService"/>. This is the only
    /// Gemini/provider-specific code — swapping to another OpenAI-compatible provider (or a real
    /// OpenAI/Azure endpoint) only requires changing <see cref="LlmOptions"/> or adding a sibling
    /// registration method.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers an <see cref="IChatClient"/> backed by the configured (Gemini) OpenAI-compatible
        /// endpoint, plus the <see cref="ILlmService"/> that uses it.
        /// </summary>
        public static IServiceCollection AddLlmService(this IServiceCollection services, LlmOptions options)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(options);

            var clientOptions = new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint) };
            IChatClient chatClient = new OpenAIClient(new ApiKeyCredential(options.ApiKey), clientOptions)
                .GetChatClient(options.Model)
                .AsIChatClient();

            services.AddChatClient(chatClient);
            services.AddSingleton<ILlmService, LlmService>();

            return services;
        }
    }
}
