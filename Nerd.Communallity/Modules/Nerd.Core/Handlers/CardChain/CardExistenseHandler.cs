using Microsoft.Extensions.Logging;
using Nerd.Core.Extensions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;
using static Nerd.Core.Extensions.ErrorExtensions;

namespace Nerd.Core.Handlers.CardChain;

public class CardExistenceHandler(ILogger<AbstractCardCheckHandler> logger) : AbstractCardCheckHandler(logger)
{
    public override CheckCardResponse Handle(CheckCardRequest request, Card? card)
    {
        if (card == null)
        {
            return logger.LogAndReturnResponse<CheckCardResponse>(OperationErrors[Errors.CardIsNotExist], Errors.CardIsNotExist);
        }

        return NextHandler.Handle(request, card!);
    }
}
