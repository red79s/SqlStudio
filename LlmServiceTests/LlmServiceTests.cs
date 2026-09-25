using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using LlmService;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LlmServiceImpl = LlmService.LlmService;

namespace LlmServiceTests
{
    [TestClass]
    public class LlmServiceTests
    {
        private Mock<IChatClient> _chatClient = new Mock<IChatClient>();
        private List<ChatMessage> _capturedMessages = new List<ChatMessage>();

        private LlmServiceImpl CreateService(string modelResponse)
        {
            _chatClient = new Mock<IChatClient>();
            _capturedMessages = new List<ChatMessage>();
            _chatClient
                .Setup(c => c.GetResponseAsync(
                    It.IsAny<IEnumerable<ChatMessage>>(),
                    It.IsAny<ChatOptions?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken>(
                    (messages, options, ct) => _capturedMessages = messages.ToList())
                .ReturnsAsync(new ChatResponse(new ChatMessage(ChatRole.Assistant, modelResponse)));

            return new LlmServiceImpl(_chatClient.Object, NullLogger<LlmServiceImpl>.Instance);
        }

        [TestMethod]
        public async Task GenerateSqlAsync_ReturnsModelText()
        {
            var service = CreateService("SELECT * FROM customers;");

            var result = await service.GenerateSqlAsync("all customers");

            Assert.AreEqual("SELECT * FROM customers;", result.Sql);
            Assert.AreEqual("SELECT * FROM customers;", result.RawResponse);
        }

        [TestMethod]
        public async Task GenerateSqlAsync_StripsMarkdownCodeFences()
        {
            var service = CreateService("```sql\nSELECT 1;\n```");

            var result = await service.GenerateSqlAsync("give me one");

            Assert.AreEqual("SELECT 1;", result.Sql);
        }

        [TestMethod]
        public async Task GenerateSqlAsync_SendsSystemAndUserMessages()
        {
            var service = CreateService("SELECT 1;");

            await service.GenerateSqlAsync("give me one");

            Assert.AreEqual(2, _capturedMessages.Count);
            Assert.AreEqual(ChatRole.System, _capturedMessages[0].Role);
            Assert.AreEqual(ChatRole.User, _capturedMessages[1].Role);
            StringAssert.Contains(_capturedMessages[1].Text, "give me one");
        }

        [TestMethod]
        public async Task GenerateSqlAsync_WithSchema_IncludesSchemaContextInPrompt()
        {
            var service = CreateService("SELECT * FROM foo;");
            var schema = new Mock<IDatabaseSchemaInfo>();
            schema.Setup(x => x.DatabaseName).Returns("MyDb");
            schema.Setup(x => x.Tables).Returns(new List<TableInfo>
            {
                new TableInfo
                {
                    TableName = "foo",
                    Columns = new List<ColumnInfo>
                    {
                        new ColumnInfo { ColumnName = "foo_col1", ColumnType = "int" }
                    }
                }
            });
            schema.Setup(x => x.ForeignKeys).Returns(new List<Common.Model.ForeignKeyInfo>());

            await service.GenerateSqlAsync("everything in foo", schema.Object);

            var userMessage = _capturedMessages[1].Text;
            StringAssert.Contains(userMessage, "foo");
            StringAssert.Contains(userMessage, "foo_col1");
            StringAssert.Contains(userMessage, "everything in foo");
        }

        [TestMethod]
        public async Task GenerateSqlAsync_EmptyRequest_Throws()
        {
            var service = CreateService("unused");

            await Assert.ThrowsAsync<ArgumentException>(
                () => service.GenerateSqlAsync("  "));
        }
    }
}
