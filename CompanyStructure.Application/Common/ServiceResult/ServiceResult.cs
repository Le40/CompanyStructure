namespace CompanyStructure.Application.Common.ServiceResult
{
    public record ServiceResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ServiceError? Error { get; init; }

        public static ServiceResult<T> Ok(T data) =>
            new() { Success = true, Data = data };

        public static ServiceResult<T> Fail(ServiceError error) =>
            new() { Success = false, Error = error };
    }

    public enum ServiceErrorType
    {
        Validation,
        NotFound,
        Conflict/*,
        Unauthorized,
        Forbidden*/
    }

    public record ServiceError(string Code, ServiceErrorType Type, string Message);

}
