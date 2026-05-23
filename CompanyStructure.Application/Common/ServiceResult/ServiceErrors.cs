namespace CompanyStructure.Application.Common.ServiceResult
{
    public static class ServiceErrors
    {
        // Company specific errors
        public static ServiceError CompanyNotFound(int id) =>
            new(
                Code: "Company.NotFound", 
                Message: "Company with id {id} does not exist.",
                Type: ServiceErrorType.NotFound);

        public static ServiceError CompanyLeaderCannotBeAssignedOnCreate =>
            new(
                Code: "Company.Validation",
                Message: "Company director cannot be assigned when creating a new company.",
                Type: ServiceErrorType.Validation);

        // Employee specific errors
        public static ServiceError EmailAlreadyExists =>
            new(
                Code: "Employee.DuplicateEmail",
                Message: "Email already exists.",
                ServiceErrorType.Conflict);

        // Generic errors
        public static ServiceError NotFound<T>(int id) =>
            new(
                Code: $"{typeof(T).Name}.NotFound", 
                Message: $"{typeof(T).Name} with id {id} not found.",
                Type: ServiceErrorType.NotFound);

        public static ServiceError DuplicateCode<T>(string code) =>
            new(
                Code: $"{typeof(T).Name}.DuplicateCode",
                Message: $"{typeof(T).Name} code {code} already exists.",
                Type: ServiceErrorType.Conflict,
                Metadata: new Dictionary<string, object?>
                {
                    ["code"] = code
                });

        public static ServiceError InvalidLeader<T>() =>
            new(
                Code: $"{typeof(T).Name}.InvalidLeader", 
                Message: $"Leader of {typeof(T).Name} must be an employee of the same company.",
                Type: ServiceErrorType.Validation);

        public static ServiceError Forbidden(string action) =>
            new(
                Code: "Authorization.Forbidden",
                Message: $"You are not allowed to {action}.",
                Type: ServiceErrorType.Forbidden);
    }
}
