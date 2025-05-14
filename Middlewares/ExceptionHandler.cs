using Practice.Models;

namespace Practice.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandler(RequestDelegate next,
            ILogger<ExceptionHandler> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ArgumentNullException:
                        context.Response.StatusCode = 400;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Invalid input."));
                        break;
                    case HttpRequestException:
                        context.Response.StatusCode = 503;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(GetErrorMessage(ex)));
                        break;
                    default:
                        _logger.LogError(ex, "Unhandled error");
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(GetErrorMessage(ex)));
                        break;
                }
            }
        }

        private string GetErrorMessage(Exception e)
        {
            if (_env.IsDevelopment())
            {
                _logger.LogError(e, e.Message);
                return e.Message;
            }

            _logger.LogError("An unhandled exception occurred: {Message}", e.Message);
            return "An unexpected error occurred. Please Contact GS1 Nigeria";
        }
    }
}
