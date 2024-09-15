using Acme.Server.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Acme.Server.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static IList<Exception> GetExceptions(this ModelStateDictionary modelState)
    {
        var exceptions = modelState.Values
            .SelectMany(x => x.Errors)
            .Where(x => x.Exception is not null)
            .Select(x => x.Exception!)
            .ToList();

        return exceptions;
    }

    public static IList<ValidationError> GetErrorMessages(this ModelStateDictionary modelState)
    {
        var messages = modelState
            .Where(x => x.Value?.Errors != null)
            .SelectMany(x => x.Value!.Errors, (x, e) => new ValidationError(e.ErrorMessage))
            .ToList();

        return messages;
    }
}
