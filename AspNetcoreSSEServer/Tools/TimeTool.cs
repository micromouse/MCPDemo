using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AspNetcoreSSEServer.Tools {
    /// <summary>
    /// TimeTool - A tool to get the current time
    /// </summary>
    [McpServerToolType]
    public class TimeTool {
        /// <summary>
        /// GetCurrentTime - Gets the current time in the specified city
        /// </summary>
        /// <param name="city">城市</param>
        /// <returns>城市时间</returns>
        [McpServerTool, Description("Gets the current time in the specified city")]
        public string GetCurrentTime([Description("specified city")]string city) {
            return $"it is {DateTime.Now:yyyy-MM-dd HH:mm:ss} in {city}";
        }
    }
}
