namespace AspNetcoreSSEServer.Application.DtoModel {
    /// <summary>
    /// LLMDto - Data Transfer Object for LLM
    /// </summary>
    public class LLMDto {
        /// <summary>
        /// Prompt - The prompt to send to the LLM
        /// </summary>
        public string Prompt { get; init; } = null!;
        /// <summary>
        /// MaxTokens - The maximum number of tokens to generate
        /// </summary>
        public int MaxTokens { get; init; }
    }
}
