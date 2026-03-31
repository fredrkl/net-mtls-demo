public class ApiKeyEndpointFilter : IEndpointFilter
{
    private readonly string _apiKey;

    public ApiKeyEndpointFilter(IConfiguration configuration)
    {
        _apiKey = configuration["Vayapay:ApiKey"] ?? string.Empty;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        if (!httpContext.Request.Headers.TryGetValue("X-API-KEY", out var key))
        {
            httpContext.Response.StatusCode = 401; // Unauthorized
            return Results.Json(new { error = "API Key is missing" });
        }

        var providedKey = key.ToString();
        if (string.IsNullOrWhiteSpace(providedKey) || providedKey != _apiKey)
        {
            httpContext.Response.StatusCode = 403; // Forbidden
            return Results.Json(new { error = "Invalid API Key" });
        }

        // Optionally, set client info in HttpContext.Items for downstream use
        httpContext.Items["client"] = "vayapay";

        return await next(context);
    }
}
