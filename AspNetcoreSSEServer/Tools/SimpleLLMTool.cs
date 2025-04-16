using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AspNetcoreSSEServer.Tools {
    /// <summary>
    /// SimpleLLMTool - A simple LLM tool
    /// </summary>
    /// <param name="logger">日志器</param>
    /// <param name="mcpServer">MCP服务器</param>
    [McpServerToolType]
    public sealed class SimpleLLMTool(ILogger<SimpleLLMTool> logger, IMcpServer mcpServer) {
        /// <summary>
        /// SimpleLLMAsync - Asynchronously samples from an LLM using MCP's sampling feature
        /// </summary>
        /// <param name="prompt">提示词</param>
        /// <param name="maxTokens">最大Token数</param>
        /// <param name="cancellationToken">取消Token</param>
        /// <returns>结果</returns>
        [McpServerTool(Name = "SimpleLLMTool"), Description("Samples from an LLM using MCP's sampling feature")]
        public async Task<string> SimpleLLMAsync(
            [Description("The prompt to send to the LLM")] string prompt,
            [Description("Maximum number of tokens to generate")] int maxTokens,
            CancellationToken cancellationToken = default
            ) {
            logger.LogInformation("SimpleLLMAsync called at {time}", DateTime.Now);

            ChatMessage[] messages = [
                new(ChatRole.System, "You are a helpful test server."),
                new(ChatRole.User, prompt),
            ];
            ChatOptions options = new() {
                MaxOutputTokens = maxTokens,
                Temperature = 0.7f,
            };

            var samplingResponse = await mcpServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
            return $"LLM sampling result: {samplingResponse}";
        }
    }
}
