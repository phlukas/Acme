using Acme.Server.Common;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Server.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result) => result.IsFailed ? result.ToFailActionResult() : new OkResult();

    public static ActionResult ToFailActionResult<T>(this Result<T> result) => result.ToResult().ToFailActionResult();

    public static ActionResult ToFailActionResult(this Result result)
    {
        var validationResponse = ValidationErrorResponse.CreateFromFluentResultErrors(result.Errors);
        return new BadRequestObjectResult(validationResponse);
    }
}
