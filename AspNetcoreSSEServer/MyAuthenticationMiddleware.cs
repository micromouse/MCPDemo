using System.Text.Json;

namespace AspNetcoreSSEServer {
    /// <summary>
    /// MyAuthenticationMiddleware - Middleware for authentication
    /// </summary>
    /// <param name="next">下一个操作</param>
    public class MyAuthenticationMiddleware(RequestDelegate next) {
        /// <summary>
        /// 异步调用中间件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context) {
            // 获取 Authorization header
            var token = context.Request.Headers.Authorization.FirstOrDefault();

            // 如果没有 Authorization 头或者无效 token
            if (string.IsNullOrEmpty(token) || token != "Bearer your-secret-token") {
                // 返回 401 Unauthorized 错误
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing authorization token.");
                return;
            }

            // 如果通过验证，继续执行下一个中间件
            await next(context);
        }
    }
}