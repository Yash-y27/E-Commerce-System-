using System.Net;
using System.Text.Json;

namespace AuthService.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new
            {
                success = false,
                message = ex.Message
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}