using Microsoft.Extensions.Logging;
using Nerd.Domain.Enums;
using static Nerd.Domain.Extensions.ErrorExtensions;

namespace Nerd.Domain.Extensions;

public static class LoggerExtensions
{
    private static void LogErrorEventAndScope(this ILogger logger, object response)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["prefixError"] = "--->Occured error:",
        })) 
        {
            logger.LogError($"{response}");
        };
    }

    public static T LogAndReturnResponse<T>(this ILogger logger, string? errorMessage = null, Errors errorCode = Errors.Success) where T : class
    {
        object response;

        if (errorCode == Errors.Success && string.IsNullOrEmpty(errorMessage) is false)
        {
            response = new
            {
                Error = Errors.Success,
                Message = OperationErrors[Errors.Success]
            };
        }
        else
        {
            response = new
            {
                Error = errorCode != Errors.Success && errorCode != Errors.RequestIsBad
                    ? errorCode
                    : Errors.RequestIsBad,
                Message = string.IsNullOrWhiteSpace(errorMessage)
                    ? OperationErrors[Errors.RequestIsBad]
                    : errorMessage
            };
            logger.LogErrorEventAndScope(response);
        }

        return (T)response;
    }

    public static void LogErrorEvent(this ILogger logger, string? errorMessage = null, Errors errorCode = Errors.Success)
    {
        object response;

        if (errorCode == Errors.Success && string.IsNullOrEmpty(errorMessage) is false)
        {
            response = new
            {
                Error = Errors.Success,
                Message = OperationErrors[Errors.Success]
            };
        }
        else
        {
            response = new
            {
                Error = errorCode != Errors.Success && errorCode != Errors.RequestIsBad
                    ? errorCode
                    : Errors.RequestIsBad,
                Message = string.IsNullOrWhiteSpace(errorMessage)
                    ? OperationErrors[Errors.RequestIsBad]
                    : errorMessage
            };
        }

        logger.LogErrorEventAndScope(response);
    }
}