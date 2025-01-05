using Microsoft.Extensions.Logging;
using TaskManagement.Domain.Common.AuditLog;

namespace TaskManagement.Application.Utilities.AuditLog
{
    /// <summary>
    /// Implements the <see cref="IActivityLog"/> interface for logging activity and errors.
    /// This class provides methods to log error messages, optionally including exception details.
    /// </summary>
    public class ActivityLog : IActivityLog
    {
        private readonly ILogger<object> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLog"/> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{T}"/> instance used for logging activities.</param>
        public ActivityLog(ILogger<object> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Logs an error message along with optional exception details.
        /// </summary>
        /// <param name="message">The custom error message to log.</param>
        /// <param name="exception">An optional exception to include in the log, which may provide additional context.</param>
        public void LogError(string message, Exception? exception = null)
        {
            var fullMessage = $"\nCustom message : {message} ";

            if (exception is null)
            {
                _logger.LogError(fullMessage);
            }
            else
            {
                fullMessage += $"\nInnerException: {exception.InnerException?.Message}. \nMessage: {exception.Message}";
                _logger.LogError(exception, fullMessage);
            }
        }
    }
}
