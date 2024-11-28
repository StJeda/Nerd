using Microsoft.Extensions.Logging;
using Nerd.Core.Extensions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;
using static Nerd.Core.Extensions.ErrorExtensions;

namespace Nerd.Core.Handlers.CardChain;

public class CardStatusHandler(ILogger<AbstractCardCheckHandler> logger) : AbstractCardCheckHandler(logger)
{
    public override CheckCardResponse Handle(CheckCardRequest request, Card card)
    {
        if (card.Status is false)
        {
            return logger.LogAndReturnResponse<CheckCardResponse>(OperationErrors[Errors.CardIsBlocked], Errors.CardIsBlocked);
        }

        return NextHandler.Handle(request, card);
    }
}
