namespace TaskManagement.Domain.Common.ReturnType
{
    /// <summary>
    /// Represents the result of an operation that can either succeed or fail.
    /// Encapsulates a value on success or an error on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned on success.</typeparam>
    public sealed class OptionResult<TValue>
    {

        private readonly TValue? _value;
        private readonly Error[] _error;

        private readonly bool _isSuccess;

        private OptionResult(TValue value)
        {
            _isSuccess = true;
            _value = value;
            _error = new Error[] { };
        }

        private OptionResult(Error[] error)
        {
            _isSuccess = false;
            _value = default;
            _error = error;
        }

        public static OptionResult<TValue> Success(TValue value) => new(value);
        public static OptionResult<TValue> Failure(Error[] error) => new(error);

        public static implicit operator OptionResult<TValue>(TValue value) => new(value);
        public static implicit operator OptionResult<TValue>(Error[] error) => new(error);

        public TResult Match<TResult>(Func<TValue, TResult> success, Func<Error[], TResult> failure)
        {
            return _isSuccess ? success(_value!) : failure(_error);
        }
    }

    public record Error(int StatusCode, string Status, string Description)
    {
        public static Error None => new(400, string.Empty, string.Empty);
    }
}
