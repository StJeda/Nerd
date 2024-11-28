using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Nerd.Core.Commands;
using Nerd.Core.Extensions;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Handlers;

public record CreateDocumentCommandHandler(IDocumentRepository repository, 
    IDebtRepository debtRepository,
    IValidator<CreateDocumentRequest> validator,
    AbstractPaymentStrategy paymentStrategy,
    IMapper mapper,
    ILogger<CreateDocumentCommandHandler> logger) : IRequestHandler<CreateDocumentCommand, CreateDocumentResponse>
{
    public async Task<CreateDocumentResponse> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            CardRequest cardRequest = mapper.Map<CardRequest>(request.PayInfo);

            DebtRecord debt = await debtRepository.GetDebtRecordByIdAsync(request.DebtSeria) ?? throw new NullReferenceException("Debt can't be null");

            if(debt.Amount > request.Amount)
            {
                throw new Exception("Debt amount < your amount");
            }
            if(debt.Amount < request.Amount)
            {
                logger.LogInformation("Your amount in other is fired");
            }

            decimal debtWithComission = debt.Amount + (debt.Amount / 100.0m * 1.5m);  //commission 

            Dictionary<string, object> controls = new()
            {
                ["Статус"] = DocumentStatus.INITIAL,
                ["Адреса"] = debt.Address,
                ["Серія комунального платежу"] = debt.DebtSeria,
                ["Опис"] = debt.DebtDescription,
                ["Поштовий індекс"] = debt.PostIndex,
                ["Сума боргу"] = debtWithComission.ToString("F2") + " " + Currency.UAH.ToString(),
                ["Дата платежу"] = DateTime.Now,
                ["Мерчант"] = request.MerchantName,
                ["Платник"] = request.PayerName
            };

            NerdPaymentRequest paymentRequest = new()
            {
                Amount = request.Amount,
                DebtSeria = request.DebtSeria,
                ExpDate = cardRequest.expDate,
                Currency = request.Currency,
                PayerCard = cardRequest.PayerCard,
                PIN = cardRequest.PIN
            };

            NerdPaymentResponse response = await paymentStrategy.ExecuteAsync(paymentRequest, controls);

            if (response.Code == Errors.Success)
            {
                (Errors cardIsCode, string cardIsMessage) = (response.Code, response.Message);

                logger.LogInformation("CardIs returned: ({code})\n\t {message}", cardIsCode, cardIsMessage);

                if (cardIsCode != (int)Errors.Success)
                {
                    throw new ArgumentException($"CardIs blocked operation: {cardIsCode} | {cardIsMessage}");
                }
            }
            else
            {
                throw new SystemException("CardIs don't give an answer");
            }

            CreateDocumentRequest documentRequest = mapper.Map<CreateDocumentRequest>(request);

            ValidationResult? validationResult = await validator.ValidateAsync(documentRequest);

            if (validationResult.IsValid is false)
            {
                return new CreateDocumentResponse()
                {
                    DocumentId = null,
                    OperationCode = validationResult.Errors.First().ErrorCode,
                    OperationMessage = validationResult.Errors.First().ErrorMessage,
                    Controls = new Dictionary<string, object>(),
                    Status = DocumentStatus.FAILED
                };
            }

            string xmlContent = XmlExtensions.SerializeDictionaryToXml(response.Controls);

            int documentCreationTask = await repository.CreateDocumentAsync(documentRequest, xmlContent);

            logger.LogInformation("Document created: {0}", documentCreationTask);

            Document? document = await repository.GetDocumentByIdAsync(request.DocumentId);

            if (document is not { })
            {
                throw new NullReferenceException("document can't be a null");
            }

            CreateDocumentResponse result = mapper.Map<CreateDocumentResponse>(document);

            await debtRepository.UpdateDebtRecordStatusAsync(request.DebtSeria, true);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogErrorEvent(ex.Message, Errors.ServerError);
            return new CreateDocumentResponse()
            {
                DocumentId = null,
                OperationCode = "500",
                OperationMessage = ex.Message,
                Controls = new Dictionary<string, object>(),
                Status = DocumentStatus.FAILED
            };
        }
    }
}
