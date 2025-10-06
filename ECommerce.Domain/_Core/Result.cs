namespace ECommerce.Domain._Core
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Não pode haver erro quando a operação é sucesso.");
            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Deve haver um erro quando a operação falha.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, string.Empty);
        public static Result Failure(string message) => new Result(false, message);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value
        {
            get
            {
                if (IsFailure)
                    throw new InvalidOperationException("Não é permitido acessar o valor de um resultado de falha.");
                return _value!;
            }
        }

        protected internal Result(T? value, bool isSuccess, string error) : base(isSuccess, error)
        {
            _value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);
        public new static Result<T> Failure(string message) => new Result<T>(default, false, message);
    }
}