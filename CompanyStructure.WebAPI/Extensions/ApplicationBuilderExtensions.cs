using Microsoft.AspNetCore.Diagnostics;

namespace CompanyStructure.WebAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppRequestLogging(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("RequestLogging");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                logger.LogInformation(
                    "Request started {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await next();

                stopwatch.Stop();

                logger.LogInformation(
                    "Request finished {Method} {Path} with {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            });

            return app;
        }

        public static IApplicationBuilder UseAppExceptionHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    logger.LogError(exceptionHandlerPathFeature?.Error,
                        "An unhandled exception occurred while processing the request {Path}.",
                        context.Request.Path);

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "An unexpected error occurred."
                    });

                });
            });

            return app;
        }
    }
}
