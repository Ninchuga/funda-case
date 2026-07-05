namespace Funda.Domain.Models
{
    public class Result
    {
        public IReadOnlyList<string> Errors { get; } = [];
        public IReadOnlyList<string> Warnings { get; } = [];
        public bool IsSuccess { get; }

        protected Result(IEnumerable<string>? errors = null, IEnumerable<string>? warnings = null)
        {
            Errors = errors?.ToList().AsReadOnly() ?? [];
            Warnings = warnings?.ToList().AsReadOnly() ?? [];
            IsSuccess = !Errors.Any();
        }

        public static Result Success(IEnumerable<string>? warnings = null)
            => new Result(errors: [], warnings ?? []);

        public static Result Failure(IEnumerable<string> errors, IEnumerable<string>? warnings = null)
            => new Result(errors, warnings);

        public static Result Failure(string error, IEnumerable<string>? warnings = null)
            => new Result([error], warnings);

    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(T? data, IEnumerable<string>? errors = null, IEnumerable<string>? warnings = null)
            : base(errors, warnings)
        {
            Data = data;
        }

        public static Result<T> Success(T value, IEnumerable<string>? warnings = null)
            => new Result<T>(value, errors: null, warnings);

        public static new Result<T> Failure(IEnumerable<string> errors, IEnumerable<string>? warnings = null)
            => new Result<T>(data: default, errors, warnings);

        public static new Result<T> Failure(string error, IEnumerable<string>? warnings = null)
            => new Result<T>(data: default, [error], warnings);
    }
}
