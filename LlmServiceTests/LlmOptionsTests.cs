using System;
using System.Linq;
using System.Threading.Tasks;
using LlmService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LlmServiceTests
{
    [TestClass]
    public class LlmOptionsTests
    {
        [TestMethod]
        public void DefaultProvider_IsOpenAiCompatible_WithGeminiDefaults()
        {
            var options = new LlmOptions();

            Assert.AreEqual(LlmProvider.OpenAiCompatible, options.Provider);
            Assert.AreEqual(LlmProviderDefaults.GeminiEndpoint, options.Endpoint);
            Assert.AreEqual("gemini-2.5-flash", options.Model);
        }

        [TestMethod]
        public void OllamaProvider_UsesOllamaDefaults()
        {
            var options = new LlmOptions { Provider = LlmProvider.Ollama };

            Assert.AreEqual(LlmProviderDefaults.OllamaEndpoint, options.Endpoint);
            Assert.AreEqual("llama3.1", options.Model);
        }

        [TestMethod]
        public void ExplicitValues_OverrideProviderDefaults()
        {
            var options = new LlmOptions
            {
                Provider = LlmProvider.Ollama,
                Model = "qwen2.5-coder",
                Endpoint = "http://gpu-box:11434/v1/"
            };

            Assert.AreEqual("qwen2.5-coder", options.Model);
            Assert.AreEqual("http://gpu-box:11434/v1/", options.Endpoint);
        }

        [TestMethod]
        public void BlankValues_FallBackToProviderDefaults()
        {
            var options = new LlmOptions { Provider = LlmProvider.Ollama, Model = "  ", Endpoint = string.Empty };

            Assert.AreEqual(LlmProviderDefaults.OllamaEndpoint, options.Endpoint);
            Assert.AreEqual("llama3.1", options.Model);
        }

        [TestMethod]
        public void RequiresApiKey_IsFalseOnlyForOllama()
        {
            Assert.IsTrue(LlmProviderDefaults.RequiresApiKey(LlmProvider.OpenAiCompatible));
            Assert.IsFalse(LlmProviderDefaults.RequiresApiKey(LlmProvider.Ollama));
        }

        [TestMethod]
        public void SuggestedModels_OfferCurrentGeminiFlashModels()
        {
            var models = LlmProviderDefaults.SuggestedModels(LlmProvider.OpenAiCompatible).ToList();

            CollectionAssert.Contains(models, "gemini-3.6-flash");
            CollectionAssert.Contains(models, "gemini-2.5-flash");
        }

        [TestMethod]
        public void SuggestedModels_ExcludeModelsGoogleHasRetired()
        {
            // Closed to new users; the API answers requests for it with 404.
            CollectionAssert.DoesNotContain(
                LlmProviderDefaults.SuggestedModels(LlmProvider.OpenAiCompatible).ToList(),
                "gemini-2.5-flash-lite");
        }

        [TestMethod]
        public void SuggestedModels_ContainTheProviderDefault()
        {
            foreach (LlmProvider provider in Enum.GetValues(typeof(LlmProvider)))
            {
                CollectionAssert.Contains(
                    LlmProviderDefaults.SuggestedModels(provider).ToList(),
                    LlmProviderDefaults.Model(provider),
                    $"The default model for {provider} should be offered in the settings UI.");
            }
        }

        [TestMethod]
        public void SuggestedModels_HaveNoDuplicates()
        {
            foreach (LlmProvider provider in Enum.GetValues(typeof(LlmProvider)))
            {
                var models = LlmProviderDefaults.SuggestedModels(provider);

                Assert.AreEqual(
                    models.Count,
                    models.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                    $"Suggestions for {provider} contain a duplicate.");
            }
        }

        [TestMethod]
        public async Task TestAsync_OpenAiCompatibleWithoutApiKey_FailsWithoutThrowing()
        {
            var result = await LlmConnectionTester.TestAsync(new LlmOptions());

            Assert.IsFalse(result.Success);
            StringAssert.Contains(result.Message, "API key");
        }

        [TestMethod]
        public async Task TestAsync_UnreachableEndpoint_ReportsFailure()
        {
            // Port 1 is reserved and never listening, so this fails at the transport layer without
            // depending on any external service.
            var result = await LlmConnectionTester.TestAsync(
                new LlmOptions
                {
                    Provider = LlmProvider.Ollama,
                    Endpoint = "http://127.0.0.1:1/v1/"
                },
                TimeSpan.FromSeconds(10));

            Assert.IsFalse(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
        }

        [TestMethod]
        public void NormalizeEndpoint_AppendsV1ForOllamaBaseUrl()
        {
            Assert.AreEqual(
                "http://localhost:11434/v1/",
                LlmProviderDefaults.NormalizeEndpoint(LlmProvider.Ollama, "http://localhost:11434"));

            Assert.AreEqual(
                "http://localhost:11434/v1/",
                LlmProviderDefaults.NormalizeEndpoint(LlmProvider.Ollama, "http://localhost:11434/"));
        }

        [TestMethod]
        public void NormalizeEndpoint_KeepsExistingV1Suffix()
        {
            Assert.AreEqual(
                "http://gpu-box:11434/v1/",
                LlmProviderDefaults.NormalizeEndpoint(LlmProvider.Ollama, "http://gpu-box:11434/v1"));

            Assert.AreEqual(
                "http://gpu-box:11434/v1/",
                LlmProviderDefaults.NormalizeEndpoint(LlmProvider.Ollama, "http://gpu-box:11434/v1/"));
        }

        [TestMethod]
        public void NormalizeEndpoint_LeavesOpenAiCompatibleEndpointsAlone()
        {
            Assert.AreEqual(
                LlmProviderDefaults.GeminiEndpoint,
                LlmProviderDefaults.NormalizeEndpoint(LlmProvider.OpenAiCompatible, LlmProviderDefaults.GeminiEndpoint));
        }

        [TestMethod]
        public void NormalizeEndpoint_EmptyValue_ReturnsProviderDefault()
        {
            Assert.AreEqual(
                LlmProviderDefaults.OllamaEndpoint,
                LlmProviderDefaults.NormalizeEndpoint(LlmProvider.Ollama, "   "));
        }

        [TestMethod]
        public void CreateChatClient_Ollama_WorksWithoutApiKey()
        {
            var client = ServiceCollectionExtensions.CreateChatClient(
                new LlmOptions { Provider = LlmProvider.Ollama });

            Assert.IsNotNull(client);
        }

        [TestMethod]
        public void CreateChatClient_OpenAiCompatibleWithoutApiKey_Throws()
        {
            Assert.Throws<InvalidOperationException>(
                () => ServiceCollectionExtensions.CreateChatClient(new LlmOptions()));
        }

        [TestMethod]
        public void CreateChatClient_OpenAiCompatibleWithApiKey_Succeeds()
        {
            var client = ServiceCollectionExtensions.CreateChatClient(
                new LlmOptions { ApiKey = "test-key" });

            Assert.IsNotNull(client);
        }
    }
}
