using Microsoft.Extensions.Logging;
using Nerd.Domain.DTOs;
using Nerd.Domain.Models;

namespace Nerd.Core.Handlers.CardChain;

public abstract class AbstractCardCheckHandler(ILogger<AbstractCardCheckHandler> logger)
{
    protected AbstractCardCheckHandler NextHandler = null!;

    public void SetNextHandler(AbstractCardCheckHandler nextHandler)
    {
        NextHandler = nextHandler;
    }

    public abstract CheckCardResponse Handle(CheckCardRequest request, Card? card);
}
