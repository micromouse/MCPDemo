using Microsoft.Extensions.AI;
using System.Text;

namespace ChatWithTool {
    /// <summary>
    /// 会话记忆管理器
    /// </summary>
    /// <param name="chatClient"><see cref="IChatClient"/></param>
    public class ConversationMemoryManager(IChatClient chatClient) {
        private string _summary = string.Empty;
        private string _currentRole = "多功能业务助手";
        private readonly int _maxMessages = 20;
        private readonly int _summarizeAfter = 12;
        private readonly List<ChatMessage> _messages = [];

        /// <summary>
        /// 获得消息集合
        /// </summary>
        /// <returns>消息集合</returns>
        public IReadOnlyList<ChatMessage> GetMessages() {
            return [
                new(ChatRole.System, this.GetSystemPrompt()),
                .._messages.TakeLast(_maxMessages).ToList()
            ];
        }

        /// <summary>
        /// 获得摘要
        /// </summary>
        /// <returns>摘要</returns>
        public string GetSummary() => _summary;

        /// <summary>
        /// 获得角色
        /// </summary>
        /// <returns>角色</returns>
        public string GetRole() => _currentRole;

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="message">消息</param>
        public void AddMessage(ChatMessage message) {
            _messages.Add(message);
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="messages">Chat响应更新集合</param>
        public void AddMessages(params ChatResponseUpdate[] messages) {
            _messages.AddMessages(messages);
        }

        /// <summary>
        /// 异步生成摘要
        /// </summary>
        /// <returns>任务</returns>
        public async Task GenerateSummaryAsync() {
            //消息数不超过摘要数，不生成摘要
            if (_messages.Count < _summarizeAfter) {
                return;
            }

            //需要生成摘要的消息
            var recentMessages = _messages.TakeLast(_summarizeAfter)
                .Select(m => $"{m.Role}:{m.Text}")
                .ToList();

            //根据消息生成摘要和角色
            _summary = await this.GenerateSummaryAsync(recentMessages);
            _currentRole = await this.GenerateRoleAsync(recentMessages);
        }

        /// <summary>
        /// 异步生成摘要
        /// </summary>
        /// <param name="messages">消息集合</param>
        /// <returns>摘要</returns>
        private async Task<string> GenerateSummaryAsync(List<string> messages) {
            var prompt = $"请阅读下面的对话内容，并用一句话总结当前用户的状态（如点了什么披萨、是否支付等）：\n{string.Join("\n", messages)}";
            var summary = await this.GetResponseAsync("你是一个智能助手，请总结用户当前状态", prompt);
            return summary;
        }

        /// <summary>
        /// 根据消息异步生成角色
        /// </summary>
        /// <param name="messages">消息集合</param>
        /// <returns>角色名称</returns>
        private async Task<string> GenerateRoleAsync(List<string> messages) {
            var prompt = $"请根据以下对话内容，判断你当前的任务角色，例如“披萨订购助手”、“销售数据助手”、“技术客服”等：\n{string.Join("\n", messages)}";
            var role = await this.GetResponseAsync("你是一个多功能智能助手，请根据用户对话判断你当前的任务角色。回答要简洁明了，比如'披萨订购助手'、'销售数据助手'、'技术客服'等", prompt);
            return role ?? _currentRole;
        }

        /// <summary>
        /// 异步获得响应
        /// </summary>
        /// <param name="name">系统角色内容</param>
        /// <param name="prompt">用户角色内容</param>
        /// <returns>响应</returns>
        private async Task<string> GetResponseAsync(string name, string prompt) {
            var messages = new List<ChatMessage> {
                new(ChatRole.System, name),
                new(ChatRole.User, prompt)
            };
            var response = await chatClient.GetResponseAsync(messages);
            return response.Text;
        }

        /// <summary>
        /// 获得系统提示
        /// </summary>
        /// <returns>系统提示</returns>
        private string GetSystemPrompt() {
            var builder = new StringBuilder();

            builder.AppendLine($"你是一个{_currentRole}。以下是用户当前上下文摘要：");
            builder.AppendLine(_summary.Length > 0 ? _summary : "（暂无摘要）");
            builder.AppendLine("请根据摘要和上下文与用户自然对话，帮助完成任务。");

            return builder.ToString();
        }
    }
}
