using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CW5.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string pathStr = context.Request.Path;
                string methodStr = context.Request.Method;
                string queryStr = context.Request.QueryString.ToString();
                string bodyStr = "";

                using (var rdr = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await rdr.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                string fileLog = "requestsLog.txt";
                string textLog =
                    "Log: "     + Environment.NewLine + 
                    "Path: "     + pathStr + Environment.NewLine +
                    "Method: "   + methodStr + Environment.NewLine + 
                    "Query "     + queryStr + Environment.NewLine +
                    "Body: "     + bodyStr + Environment.NewLine + Environment.NewLine;

                if (!File.Exists(fileLog))
                {
                    File.WriteAllText(fileLog, textLog);
                }
                else
                {
                    File.AppendAllText(fileLog, textLog);
                }
            }

            if (_next != null) await _next(context);
        }

    }
}