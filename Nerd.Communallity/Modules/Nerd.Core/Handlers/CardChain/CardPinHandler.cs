using Microsoft.Extensions.Logging;
using Nerd.Core.Extensions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;
using static Nerd.Core.Extensions.ErrorExtensions;

namespace Nerd.Core.Handlers.CardChain;

public class CardPinHandler(ILogger<AbstractCardCheckHandler> logger) : AbstractCardCheckHandler(logger)
{
    public override CheckCardResponse Handle(CheckCardRequest request, Card card)
    {
        Errors error = card.PIN != request.PIN ? Errors.UncorrectedPin : Errors.Success;
        return logger.LogAndReturnResponse<CheckCardResponse>(OperationErrors[error], error);
    }
}