namespace CompanyStructure.Application.Common.ServiceResult
{
    public static class ServiceErrors
    {
        // Company specific errors
        public static ServiceError CompanyNotFound =>
            new("Company.NotFound", ServiceErrorType.NotFound, "Company does not exist.");

        public static ServiceError CompanyLeaderCannotBeAssignedOnCreate =>
            new("Company.Validation", ServiceErrorType.Validation, "Company director cannot be assigned when creating a new company. Create employees first, then update company leader.");

        // Employee specific errors
        public static ServiceError EmailAlreadyExists =>
            new("Employee.DuplicateEmail", ServiceErrorType.Conflict, "Email already exists.");

        // Generic errors
        public static ServiceError NotFound<T>() =>
            new($"{typeof(T).Name}.NotFound", ServiceErrorType.NotFound, $"{typeof(T).Name} not found.");

        public static ServiceError DuplicateCode<T>() =>
            new($"{typeof(T).Name}.DuplicateCode", ServiceErrorType.Conflict, $"{typeof(T).Name} code already exists.");

        public static ServiceError LeaderIsNotEmployee<T>() =>
            new($"{typeof(T).Name}.LeaderIsNotEmployee", ServiceErrorType.Validation, $"Leader of {typeof(T).Name} must be an employee of the same company.");
    }
}
