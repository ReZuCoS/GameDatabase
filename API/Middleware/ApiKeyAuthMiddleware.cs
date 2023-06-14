namespace API.Middleware
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("ApiKey", out var requestApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync("API key is not provided in request headers");
                return;
            }

            var apiKey = _configuration.GetValue<string>("Authentication:ApiKey");

            if (!requestApiKey.Equals(apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync("Invalid API key");
                return;
            }

            await _next(context);
        }
    }
}
