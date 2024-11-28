using Nerd.Domain.DTOs;
using Nerd.Domain.Models;

namespace Nerd.Core.Handlers.CardChain;

public class CardCheckHandlerChain(CardExistenceHandler cardExistenceHandler,
    CardStatusHandler cardStatusHandler,
    CardPinHandler cardPinHandler)
{

    public AbstractCardCheckHandler CreateChain()
    {
        cardExistenceHandler.SetNextHandler(cardStatusHandler);
        cardStatusHandler.SetNextHandler(cardPinHandler);

        return cardExistenceHandler;
    }

    public CheckCardResponse StartChain(CheckCardRequest request, Card? card)
    {
        AbstractCardCheckHandler handler = CreateChain();
        return handler.Handle(request, card);
    }
}
