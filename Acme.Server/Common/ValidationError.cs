using FluentResults;

namespace Acme.Server.Common;

public class ValidationError
{
    public string Message { get; set; }

    public ValidationError()
    {
    }

    public ValidationError(string message)
    {
        Message = message;
    }

    public static ValidationError CreateFromFluentResultError(IError error)
    {
        var validationError = new ValidationError
        {
            Message = error.Message,
        };

        return validationError;
    }
}
