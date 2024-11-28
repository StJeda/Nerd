using MediatR;
using Microsoft.Extensions.Logging;
using Nerd.Core.Commands;
using Nerd.Core.Extensions;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;

namespace Nerd.Infrastructure.Handlers;

public record FillCardByCardCommandHandler(ICardIsRepository repository,
    ILogger<FillCardByCardCommandHandler> logger) : IRequestHandler<FillCardByCardCommand, PayMoneyResponse>
{
    public async Task<PayMoneyResponse> Handle(FillCardByCardCommand request, CancellationToken cancellationToken)
    {
        Currency currency = request.Currency;

        bool isEnemyCurrency = currency != Currency.RUB || currency != Currency.BLR ? true : false;

        if (isEnemyCurrency is false)
        {
            string errorString = $"Currency {currency} is enemy!";

            logger.LogInformation(errorString);

            throw new Exception(errorString);
        }

        try
        {
            decimal amountUah = request.Amount;
            switch (request.Currency)
            {
                case Currency.UAH:
                    break;
                case Currency.USD:
                    amountUah *= 40;
                    break;
            }

            await repository.TransferFundsAsync(request.PayerCard, request.GetterCard, request.Amount);
        }
        catch (Exception ex)
        {
            return logger.LogAndReturnResponse<PayMoneyResponse>(ex.Message);
        }

        return logger.LogAndReturnResponse<PayMoneyResponse>();
    }
}

