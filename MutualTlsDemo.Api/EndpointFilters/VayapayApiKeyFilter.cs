namespace MutualTlsDemo.Api.EndpointFilters;

public class VayapayApiKeyFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        throw new NotImplementedException();
    }
}
