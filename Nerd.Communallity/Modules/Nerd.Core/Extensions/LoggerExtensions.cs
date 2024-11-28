using Microsoft.Extensions.Logging;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using System.Text.Json;
using static Nerd.Core.Extensions.ErrorExtensions;

namespace Nerd.Core.Extensions;

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

        if (errorCode == Errors.Success && (errorMessage == OperationErrors[Errors.Success] || string.IsNullOrEmpty(errorMessage) is true))
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

        string json = JsonSerializer.Serialize(response);
        return JsonSerializer.Deserialize<T>(json)!;
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