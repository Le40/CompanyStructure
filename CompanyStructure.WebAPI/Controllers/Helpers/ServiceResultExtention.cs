using CompanyStructure.Application;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Helpers
{
    public static class ServiceResultExtention
    {
        public static IActionResult ToActionResult<T>(
            this ServiceResult<T> result,
            ControllerBase controller)
        {
            if (result.Success)
                return controller.Ok(result.Data);

            return result.ErrorType switch
            {
                ServiceErrorType.NotFound =>
                    controller.NotFound(new { message = result.Error }),

                ServiceErrorType.Conflict =>
                    controller.Conflict(new { message = result.Error }),

                _ => controller.BadRequest(new { message = result.Error })
            };
        }
    }
}
