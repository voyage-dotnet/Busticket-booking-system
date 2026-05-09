using BusTicketSystem.Web.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BusTicketSystem.Web.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                );

            var response = ApiResponse<object>.ErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                errors
            );

            context.Result = new BadRequestObjectResult(response);
        }
    }
}
