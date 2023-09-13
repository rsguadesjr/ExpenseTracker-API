using ExpenseTracker.Model.Common;
using System.Net;
using System.Text.Json;

namespace ExpenseTracker.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _next = next;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new CustomResponse();
            switch(exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Token"))
                        context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                    else
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    errorResponse.Message = ex.Message;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = _env.IsDevelopment() ? exception.Message : "Internal server error!";
                    errorResponse.Details = _env.IsDevelopment() ? exception.StackTrace : null;
                    break;

            }

            _logger.LogError(exception.Message, exception.StackTrace);

            errorResponse.StatusCode = context.Response.StatusCode;
            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(result);
          
        }
    }
}
