using TaskManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security;
using System.Security.Authentication;
using TaskManagement.Domain.Common.AuditLog;

namespace TaskManagement.Infrastructure.ExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IActivityLog _activityLog;

        public GlobalExceptionHandler(IActivityLog activityLog)
        {
            _activityLog = activityLog;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            _activityLog.LogError($"MachineName: {Environment.MachineName}. TraceId: {traceId}", exception);

            var (statusCode, title) = MapException(exception);

            await Results.Problem(
                title: title,
                statusCode: statusCode,
                extensions: new Dictionary<string, object>
                {
                    { "traceId", traceId }
                }).ExecuteAsync(httpContext);

            return true;

            //var details = new ProblemDetails()
            //{
            //    Detail = exception.Message,
            //    Instance = exception.StackTrace,
            //    Status = (int)HttpStatusCode.InternalServerError,
            //    Title = exception.Message,
            //    Type = "Server Error"
            //};

            //var response = JsonSerializer.Serialize(details);

            //httpContext.Response.ContentType = "application/json";
            //await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            //return true;
        }


        private static (int StatusCode, string Title) MapException(Exception ex)
        {
            // Map exceptions to specific HTTP status codes and error messages.
            return ex switch
            {
                #region Client-side errors (400 series)

                #region Bad Request

                ArgumentNullException => (StatusCodes.Status400BadRequest, ex.Message), // (Bad Request) When a required parameter is missing or null
                ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, ex.Message), // (Bad Request) When an argument is outside allowable range
                ArgumentException => (StatusCodes.Status400BadRequest, ex.Message), // (Bad Request) General invalid argument error
                FormatException => (StatusCodes.Status400BadRequest, ex.Message), //(Bad Request)  When data has an unexpected or invalid format
                InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message), // (Bad Request) When a method call is invalid for the object's state

                #endregion

                KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message), // (Not found) When a requested resource could not be found
                NotSupportedException => (StatusCodes.Status405MethodNotAllowed, ex.Message), // When an unsupported operation is attempted
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, ex.Message), // When access to a resource is unauthorized
                AuthenticationException => (StatusCodes.Status401Unauthorized, ex.Message), // (Invalid token) When a user fails authentication
                SecurityException => (StatusCodes.Status403Forbidden, ex.Message), // When a security policy violation occurs (e.g., permissions issues)
                TimeoutException => (StatusCodes.Status408RequestTimeout, ex.Message), // When an operation exceeds the allowable time limit

                #endregion

                #region Custom ConflictException

                ConflictException => (StatusCodes.Status409Conflict, ex.Message), // Custom: when a conflict with the current resource state occurs (e.g., duplicate entries)

                #endregion

                #region Database-related exceptions (500 series)

                // Uncomment SqlException only if Microsoft.Data.SqlClient is added.
                // SqlException => (StatusCodes.Status500InternalServerError, "Database error occurred."), // When there is an issue with the SQL database (e.g., query error)
                DbUpdateException => (StatusCodes.Status500InternalServerError, ex.Message), // When an error occurs while updating the database

                #endregion

                #region Server-side errors (500 series)

                // Specific exceptions are placed before their base class, SystemException, to avoid CS8510 error.
                NotImplementedException => (StatusCodes.Status501NotImplemented, ex.Message), // When a called method or feature is not yet implemented
                IOException => (StatusCodes.Status500InternalServerError, ex.Message), // When a file, network, or other I/O operation fails
                InvalidCastException => (StatusCodes.Status500InternalServerError, ex.Message), // When a type casting operation fails
                StackOverflowException => (StatusCodes.Status500InternalServerError, ex.Message), // When the execution stack overflows due to excessive recursion
                OutOfMemoryException => (StatusCodes.Status500InternalServerError, ex.Message), // When the system runs out of memory for the application
                SystemException => (StatusCodes.Status500InternalServerError, ex.Message), // General system-level errors (added last in the 500-series)

                #endregion

                // Unhandled exception
                _ => (StatusCodes.Status500InternalServerError, ex.Message) // Default case for any unhandled or unknown exception
            };
        }
    }
}
