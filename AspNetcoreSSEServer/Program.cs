using AspNetcoreSSEServer.Tools;

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
            builder.Services
                .AddMcpServer()
                .WithHttpTransport()
                .WithStdioServerTransport()
                .WithToolsFromAssembly(typeof(SimpleLLMTool).Assembly);
            
            builder.Services.AddSingleton<SimpleLLMTool>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.UseSwagger().UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.MapMcp();

            app.Run();
        }
    }
}
