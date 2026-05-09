using CompanyStructure.Application.Common.ServiceResult;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Helpers
{
    public static class ServiceResultExtension
    {
        public static IActionResult ToActionResult<T>(
            this ServiceResult<T> result,
            ControllerBase controller)
        {
            if (result.Success)
                return controller.Ok(result.Data);

            if (result.Error == null)
                return controller.StatusCode(500, new { message = "Unknown Error." });

            return result.Error.Type switch
            {
                ServiceErrorType.NotFound =>
                    controller.NotFound(new {code = result.Error.Code,message = result.Error.Message}),

                ServiceErrorType.Conflict =>
                    controller.Conflict(new { code = result.Error.Code, message = result.Error.Message }),

                ServiceErrorType.Validation =>
                    controller.BadRequest(new { code = result.Error.Code, message = result.Error.Message }),

                /*ServiceErrorType.Unauthorized =>
                    controller.Unauthorized(new { code = result.Error.Code, message = result.Error.Message }),

                ServiceErrorType.Forbidden =>
                    controller.StatusCode(403, new { code = result.Error.Code, message = result.Error.Message }),*/

                _ =>
                    controller.BadRequest(new { code = result.Error.Code, message = result.Error.Message }),
            };
        }
    }
}
