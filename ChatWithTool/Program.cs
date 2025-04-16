using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OpenAI;
using System.ClientModel;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ChatWithTool {
    /// <summary>
    /// Program - Entry point for the application
    /// </summary>
    internal class Program {
        /// <summary>
        /// Main method - Entry point for the application
        /// </summary>
        /// <param name="_">参数</param>
        static async Task Main(string[] _) {
            Console.WriteLine("Connecting client to MCP 'http://localhost:5172' server!");

            // Create a chat client using OpenAI API
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            using var chatClient = CreateChatClient(loggerFactory, GetCredentialSetting());
            var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            // Get all available tools
            var tools = await GetMcpClientToolsAsync(chatClient, loggerFactory);

            var messages = new List<ChatMessage>();
            while (true) {
                Console.Write("Q: ");
                messages.Add(new(ChatRole.User, Console.ReadLine()));

                var updates = new List<ChatResponseUpdate>();
                await foreach (var update in chatClient.GetStreamingResponseAsync(messages, new() { Tools = [.. tools] })) {
                    Console.Write(update.Text);
                    updates.Add(update);
                }
                messages.AddMessages(updates);

                /*
                // Process function calls
                var functionCalls = updates
                    .SelectMany(update => update.Contents)
                    .OfType<FunctionCallContent>()
                    .ToList();
                foreach (var functionCall in functionCalls) {
                    var tool = tools.FirstOrDefault(t => t.Name == functionCall.Name);
                    if (tool != null) {
                        var args = new AIFunctionArguments(functionCall.Arguments);
                        var result = await tool.InvokeAsync(args, CancellationToken.None);
                        Console.WriteLine($"Tool {tool.Name} result: {JsonSerializer.Serialize(result, jsonSerializerOptions)}");

                        // Send the result back to the chat client
                        messages.Add(new ChatMessage(ChatRole.Tool, result?.ToString()) {
                            AdditionalProperties = new AdditionalPropertiesDictionary {
                                ["tool_call_id"] = functionCall.CallId
                            }
                        });
                    }
                }
                */

                Console.WriteLine();
            }

        }

        /// <summary>
        /// GetCredentialSetting - Retrieves the credential settings from a JSON file
        /// </summary>
        /// <returns>凭据设置</returns>
        private static CredentialSetting GetCredentialSetting() {
            var json = File.ReadAllText("credential.json");
            return JsonSerializer.Deserialize<CredentialSetting>(json) ?? throw new Exception("Failed to deserialize credential.json");
        }

        /// <summary>
        /// CreateChatClient - Creates a chat client using OpenAI API
        /// </summary>
        /// <param name="loggerFactory">日志器工厂</param>
        /// <returns><see cref="IChatClient"/></returns>
        private static IChatClient CreateChatClient(ILoggerFactory loggerFactory, CredentialSetting credentialSetting) {
            var options = new OpenAIClientOptions {
                Endpoint = new Uri("https://api.deepseek.com/v1")
            };
            var credential = new ApiKeyCredential(credentialSetting.ApiKey);
            var openAIClient = new OpenAIClient(credential, options).GetChatClient("deepseek-chat");

            return openAIClient.AsIChatClient()
                .AsBuilder()
                .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
                .UseFunctionInvocation(loggerFactory: loggerFactory)
                .Build();
        }

        /// <summary>
        /// GetMcpClientToolsAsync - Asynchronously retrieves all available tools from the MCP Server
        /// </summary>
        /// <param name="chatClient">Chat客户端</param>
        /// <param name="loggerFactory">日志器工厂</param>
        /// <returns>Mcp客户工具集合</returns>
        private static async Task<IEnumerable<McpClientTool>> GetMcpClientToolsAsync(IChatClient chatClient, ILoggerFactory loggerFactory) {
            var mcpClient = await McpClientFactory.CreateAsync(
                new SseClientTransport(new SseClientTransportOptions {
                    Endpoint = new Uri("http://localhost:5172/")
                }),
                clientOptions: new() {
                    Capabilities = new() { Sampling = new() { SamplingHandler = chatClient.CreateSamplingHandler() } },
                },
                loggerFactory: loggerFactory);

            // Get all available tools
            Console.WriteLine("Tools available:");
            var tools = await mcpClient.ListToolsAsync();
            foreach (var tool in tools) {
                Console.WriteLine($"  {tool}");
            }

            return tools;
        }

    }
}
