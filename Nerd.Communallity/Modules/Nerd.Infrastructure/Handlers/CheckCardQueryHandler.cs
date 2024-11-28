using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Nerd.Core.Extensions;
using Nerd.Core.Handlers.CardChain;
using Nerd.Core.Queries;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Handlers;

public record CheckCardQueryHandler(ICardIsRepository repository,
    IValidator<CheckCardRequest> validator,
    ILogger<CheckCardQueryHandler> logger,
    IMapper mapper,
    CardCheckHandlerChain handlerChain) : IRequestHandler<CheckCardQuery, CheckCardResponse>
{
    public async Task<CheckCardResponse> Handle(CheckCardQuery request, CancellationToken cancellationToken)
    {
        CheckCardRequest checkRequest = mapper.Map<CheckCardRequest>(request);

        ValidationResult validationResult = validator.Validate(checkRequest);

        if (validationResult.IsValid is false)
        {
            string errorValidation = validationResult.Errors.First().ErrorMessage;
            return logger.LogAndReturnResponse<CheckCardResponse>(errorValidation);
        }

        Card? card = await repository.GetCardAsync(request.PayerCard);

        CheckCardResponse chainResponse = handlerChain.StartChain(checkRequest, card);

        return chainResponse;
    }
}
