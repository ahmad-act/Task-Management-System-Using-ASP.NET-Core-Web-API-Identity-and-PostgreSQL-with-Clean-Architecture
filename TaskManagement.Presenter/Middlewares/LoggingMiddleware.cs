using System.Text;
using Microsoft.AspNetCore.Http;

namespace TaskManagement.Presenter.Middlewares
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly string _logFilePath = "request_logs.txt";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Capture request details
            var senderIp = context.Connection.RemoteIpAddress?.ToString();
            var method = context.Request.Method;
            var uri = context.Request.Path + context.Request.QueryString;
            var timestamp = DateTime.UtcNow;

            string requestBody = string.Empty;

            // Read request body if method is POST, PUT, or PATCH
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Patch)
            {
                context.Request.EnableBuffering(); // Allow re-reading the stream

                using var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // Reset position for further processing
            }
            else if (context.Request.QueryString.HasValue)
            {
                requestBody = context.Request.QueryString.Value;
            }

            // Log the details to a file
            var logMessage = new StringBuilder();
            logMessage.AppendLine("------ Request Log ------");
            logMessage.AppendLine($"Timestamp: {timestamp:O}");
            logMessage.AppendLine($"Sender: {senderIp}");
            logMessage.AppendLine($"Method: {method}");
            logMessage.AppendLine($"URI: {uri}");
            logMessage.AppendLine("Request Data:");
            logMessage.AppendLine(requestBody);
            logMessage.AppendLine("-------------------------");

            await LogToFileAsync(logMessage.ToString());

            // Continue to the next middleware in the pipeline
            await next(context);
        }

        private async Task LogToFileAsync(string logMessage)
        {
            // Append the log message to the file asynchronously
            await File.AppendAllTextAsync(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}