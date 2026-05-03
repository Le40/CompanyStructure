using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application
{
    public enum ServiceErrorType
    {
        Validation,
        NotFound,
        Conflict
    }
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }
        public ServiceErrorType? ErrorType { get; set; }

        public static ServiceResult<T> Ok(T data) =>
            new() { Success = true, Data = data };

        public static ServiceResult<T> Fail(string error, ServiceErrorType errorType) =>
            new() { Success = false, Error = error, ErrorType = errorType };
    }
}
