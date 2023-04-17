using Microsoft.AspNetCore.Builder;

namespace ImageServer
{
    public static class ImageMiddlewareExtensions
    {
        public static IApplicationBuilder UseImageMiddleware(this IApplicationBuilder app, string rootPath)
        {
            return app.UseMiddleware<ImageMiddleware>(rootPath);
        }
    }
}
