using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class CachingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public CachingMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string file = Path.Combine(context.Request.PathBase, context.Request.Path);
        string filename = Path.GetFileName(file);
        string extension = Path.GetExtension(file);

        CachingSection config = (CachingSection)_configuration.GetSection("Websters:Caching");
        if (config != null)
        {
            context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                MaxAge = config.CachingTimeSpan,
                //Expires = DateTime.Now.Add(config.CachingTimeSpan),
                Public = true,
                MustRevalidate = false,
                NoCache = false,
                NoStore = false,
                Private = false
            };

            FileExtension fileExtension = config.FileExtensions[extension];
            if (fileExtension != null)
            {
                context.Response.ContentType = fileExtension.ContentType;
            }
        }

        context.Response.Headers.Add("content-disposition", new StringValues("inline; filename=" + filename));
        await context.Response.SendFileAsync(file);
    }
}

public class CachingSection
{
    public TimeSpan CachingTimeSpan { get; set; }
    public Dictionary<string, FileExtension> FileExtensions { get; set; }
}

public class FileExtension
{
    public string ContentType { get; set; }
}
