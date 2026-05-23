using CompanyStructure.Application.Common.ServiceResult;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Extensions
{
    public static class ServiceResultExtension
    {
        public static IActionResult ToActionResult<T>(
            this ServiceResult<T> result,
            ControllerBase controller)
        {
            if (result.Success)
                return controller.Ok(result.Data);

            return controller.ToProblem(result.Error!);
        }

        public static IActionResult ToActionResult(
            this ServiceResult result,
            ControllerBase controller)
        {
            if (result.Success)
                return controller.NoContent();

            return controller.ToProblem(result.Error!);
        }

        private static IActionResult ToProblem(
            this ControllerBase controller,
            ServiceError error)
        {
            var statusCode = error.Type switch
            {
                ServiceErrorType.Validation => StatusCodes.Status400BadRequest,
                ServiceErrorType.NotFound => StatusCodes.Status404NotFound,
                ServiceErrorType.Conflict => StatusCodes.Status409Conflict,
                ServiceErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ServiceErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return controller.Problem(
                statusCode: statusCode,
                title: error.Type.ToString(),
                detail: error.Message,
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = error.Code,
                    ["metadata"] = error.Metadata
                });
        }
    }
}
