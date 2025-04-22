using AspNetcoreSSEServer.Tools;
using Microsoft.IdentityModel.Tokens;

namespace AspNetcoreSSEServer {
    /// <summary>
    /// MCPDemo - AspNetCore Server-Sent Events (SSE) Server
    /// </summary>
    public class Program {
        /// <summary>
        /// Main method - Entry point for the application
        /// </summary>
        /// <param name="args">参数</param>
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi().AddSwaggerGen();

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://your-auth-server.com"; // 你的身份认证服务器
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateAudience = false
                    };
                });
            builder.Services.AddAuthorization();

            builder.Services
                .AddMcpServer()
                .WithHttpTransport()
                .WithStdioServerTransport()
                .WithToolsFromAssembly(typeof(SimpleLLMTool).Assembly);
            builder.Services.AddSingleton<SimpleLLMTool>();

            var app = builder.Build();
            app.UseRouting();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.UseSwagger().UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapMcp();

            app.Run();
        }
    }
}
