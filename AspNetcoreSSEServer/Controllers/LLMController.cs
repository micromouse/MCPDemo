using AspNetcoreSSEServer.Application.DtoModel;
using AspNetcoreSSEServer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace AspNetcoreSSEServer.Controllers {
    /// <summary>
    /// LLMController - A controller for handling LLM requests
    /// </summary>
    /// <param name="simpleLLMTool"><see cref="SimpleLLMTool"/></param>
    [ApiController]
    [Route("api/[controller]")]
    public class LLMController(SimpleLLMTool simpleLLMTool) : ControllerBase {
        /// <summary>
        /// InvokeLLM - Invokes the LLM with a given prompt and max tokens
        /// </summary>
        /// <param name="dto">LLM请求</param>
        /// <returns>结果</returns>
        [HttpPost("invoke")]
        public async Task<IActionResult> InvokeLLM(LLMDto dto) {
            var result = await simpleLLMTool.SimpleLLMAsync(dto.Prompt, dto.MaxTokens);
            return Ok(result);
        }
    }
}
