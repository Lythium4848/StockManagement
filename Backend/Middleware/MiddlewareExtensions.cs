namespace Backend.Middleware;

public static class MiddlewareExtensions
{
    public static RouteHandlerBuilder UseAuthenticatedMiddleware(this RouteHandlerBuilder builder)
    {
        builder.Add(endpointBuilder =>
        {
            var originalRequestDelegate = endpointBuilder.RequestDelegate;
            endpointBuilder.RequestDelegate = async context =>
            {
                var middleware = new AuthenticatedMiddleware(originalRequestDelegate!);
                await middleware.InvokeAsync(context);
            };
        });

        return builder;
    }

    public static RouteHandlerBuilder UseUnauthenticatedMiddleware(this RouteHandlerBuilder builder)
    {
        builder.Add(endpointBuilder =>
        {
            var originalRequestDelegate = endpointBuilder.RequestDelegate;
            endpointBuilder.RequestDelegate = async context =>
            {
                var middleware = new UnauthenticatedMiddleware(originalRequestDelegate!);
                await middleware.InvokeAsync(context);
            };
        });

        return builder;
    }
}