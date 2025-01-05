namespace TaskManagement.Domain.Common.AuditLog
{
    /// <summary>
    /// Defines a contract for logging activities and errors within the application.
    /// This interface provides methods to log error messages and exceptions,
    /// helping in monitoring and troubleshooting the application behavior.
    /// </summary>
    public interface IActivityLog
    {
        /// <summary>
        /// Logs an error message along with an optional exception.
        /// This method captures information about errors that occur during the application's execution.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="exception">An optional <see cref="Exception"/> object that contains additional details about the error.</param>
        void LogError(string message, Exception? exception = null);
    }
}
