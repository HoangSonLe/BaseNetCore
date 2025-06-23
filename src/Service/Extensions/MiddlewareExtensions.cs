using Application.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Application.Extensions
{
    /// <summary>
    /// Extension methods for registering custom middleware
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds global exception handling middleware to the pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }

        /// <summary>
        /// Adds all custom middleware in the correct order
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="enablePerformanceMonitoring">Whether to enable performance monitoring</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseCustomMiddleware(
            this IApplicationBuilder app)
        {
            // Global exception handling should be early in the pipeline
            app.UseGlobalExceptionHandling();

            return app;
        }
    }
}
