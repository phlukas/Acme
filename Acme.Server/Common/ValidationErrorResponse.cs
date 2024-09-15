using FluentResults;

namespace Acme.Server.Common;

public class ValidationErrorResponse
{
    public ValidationError[] Errors { get; set; }

    public static ValidationErrorResponse CreateFromFluentResultErrors(IList<IError> errors) =>
        new()
        {
            Errors = errors.Select(ValidationError.CreateFromFluentResultError).ToArray()
        };
}
