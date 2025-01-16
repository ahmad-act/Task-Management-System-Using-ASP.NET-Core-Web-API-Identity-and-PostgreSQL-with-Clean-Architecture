namespace TaskManagement.Domain.Common.ReturnType
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public Error[]? Errors { get; set; }

        public static ApiResponse Success(object? data, string message = "Operation successful")
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                Errors = null
            };
        }

        public static ApiResponse Failure(Error[]? errors, string message = "Operation failed")
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Data = null,
                Message = message,
                Errors = errors
            };
        }
    }

}
