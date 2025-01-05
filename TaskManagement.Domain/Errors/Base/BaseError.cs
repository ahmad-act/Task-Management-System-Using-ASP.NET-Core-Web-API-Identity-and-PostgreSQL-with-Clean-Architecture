using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Errors.Base
{
    public abstract class BaseError<T>
    {
        // General Errors
        public static readonly Error NotFound = new(404, "NOT_FOUND", $"{typeof(T).Name} not found.");
        public static readonly Error MissingId = new(400, "MISSING_ID", $"{typeof(T).Name} ID is required.");
        public static readonly Error MissingUniqueProperty = new(400, "MISSING_UNIQUE_PROPERTY", $"{typeof(T).Name} unique property value is required.");
        public static readonly Error Null = new(400, "NULL", $"{typeof(T).Name} cannot be null.");
        public static readonly Error InvalidData = new(400, "INVALID_DATA", $"Invalid data for {typeof(T).Name}.");
        public static readonly Error ValidationFailed = new(422, "VALIDATION_FAILED", $"Validation failed for {typeof(T).Name}.");
        public static readonly Error AlreadyExists = new(409, "ALREADY_EXISTS", $"{typeof(T).Name} already exists.");
        public static readonly Error InvalidStatus = new(400, "INVALID_STATUS", $"Invalid {typeof(T).Name} status.");
        public static readonly Error UnauthorizedAccess = new(403, "UNAUTHORIZED_ACCESS", $"Access to {typeof(T).Name} is forbidden.");

        // Title Errors
        public static readonly Error MissingTitle = new(400, "MISSING_TITLE", $"{typeof(T).Name} title is required.");
        public static readonly Error TitleTooLong = new(400, "TITLE_TOO_LONG", $"{typeof(T).Name} title is too long.");
        public static readonly Error TitleTooShort = new(400, "TITLE_TOO_SHORT", $"{typeof(T).Name} title is too short.");

        // Database Operation Errors
        public static readonly Error NoRowsAffected = new(400, "NO_ROWS_AFFECTED", $"No {typeof(T).Name} updated. It may not exist or is already in the desired state.");

        // Automapper Operation Errors
        public static readonly Error ConversionFailed = new(500, "CONVERSION_FAILED", $"Conversion of {typeof(T).Name} failed.");
    }
}
