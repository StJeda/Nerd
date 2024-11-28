using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;
using static Nerd.Core.Extensions.ErrorExtensions;

namespace Nerd.Core.Strategies;

public class CardNerdStrategy(
    IHttpClientFactory factory,
    ILogger<CardNerdStrategy> logger,
    IMessageSender sender) : AbstractPaymentStrategy
{
    public override async Task<NerdPaymentResponse> ConfirmAsync(string cardNumber, string PIN, DateTime expDate, Dictionary<string, object> controls)
    {
        try
        {
            logger.LogInformation("Starting ConfirmAsync for card {cardNumber} with expiration date {expDate}.", cardNumber, expDate);

            var checkRequest = new CheckCardRequest(cardNumber, PIN, expDate);
            var cardIsResponse = await SendCardApiRequestAsync<CheckCardResponse>(
                "https://localhost:7205/microservices/cardis/checkCard", checkRequest);

            if (cardIsResponse.Code != (int)Errors.Success)
            {
                logger.LogWarning("CardIs response code {code}: {message}", cardIsResponse.Code, cardIsResponse.Message);
                throw new ArgumentException($"CardIs blocked operation: {cardIsResponse.Code} | {cardIsResponse.Message}");
            }

            string maskedCardNumber = cardNumber.Substring(0, 8) + new string('*', 4) + cardNumber.Substring(12);
            logger.LogInformation("Card number confirmed: {maskedCardNumber}", maskedCardNumber);

            controls["Статус"] = DocumentStatus.CONFIRMED.ToString();
            controls["Картка платника"] = maskedCardNumber;
            controls["PIN"] = "***";
            controls["yyMM"] = expDate;
            
            logger.LogInformation("ConfirmAsync completed successfully for card {maskedCardNumber}.", maskedCardNumber);

            return new NerdPaymentResponse()
            {
                Code = Errors.Success,
                Message = OperationErrors[Errors.Success],
                Controls = controls
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during ConfirmAsync.");
            return HandleException(ex);
        }
    }

    public override async Task<NerdPaymentResponse> ExecuteAsync(NerdPaymentRequest request, Dictionary<string, object> initialControls)
    {
        try
        {
            logger.LogInformation("Starting ExecuteAsync for card {cardNumber} with amount {amount} {currency}.", request.PayerCard, request.Amount, request.Currency);

            NerdPaymentResponse confirmResult = await ConfirmAsync(request.PayerCard, request.PIN, request.ExpDate, initialControls);
            if (confirmResult.Code != Errors.Success)
            {
                return GetErrorResponse(confirmResult.Code);
            }

            NerdPaymentResponse paymentResult = await PayAsync(request.PayerCard, request.Amount, request.Currency, confirmResult.Controls);
            if (paymentResult.Code != Errors.Success)
            {
                return GetErrorResponse(paymentResult.Code);
            }

            return await FinalAsync(paymentResult.Controls);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during ExecuteAsync.");
            return HandleException(ex);
        }
    }

    private async Task<TResponse> SendCardApiRequestAsync<TResponse>(string url, object requestData)
    {
        logger.LogInformation("Sending request to {url} with data {requestData}.", url, JsonSerializer.Serialize(requestData));

        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        HttpClient httpClient = factory.CreateClient("CardIs");
        HttpResponseMessage response = await httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            
            TResponse? apiResponse = JsonSerializer.Deserialize<TResponse>(responseContent);
            if (apiResponse == null)
            {
                logger.LogError("Failed to deserialize response from {url}.", url);
                throw new ArgumentNullException($"Failed to deserialize response from {url}");
            }

            logger.LogInformation("Received successful response from {url}.", url);
            return apiResponse;
        }
        else
        {
            logger.LogError("API request to {url} failed with status code {statusCode}.", url, response.StatusCode);
            throw new SystemException($"API request to {url} failed");
        }
    }

    private NerdPaymentResponse GetErrorResponse(Errors errorCode)
    {
        logger.LogWarning("Returning error response with code {errorCode}.", errorCode);
        return new NerdPaymentResponse()
        {
            Code = errorCode,
            Message = OperationErrors[errorCode],
            Controls = new Dictionary<string, object>()
        };
    }

    private NerdPaymentResponse HandleException(Exception ex)
    {
        logger.LogError(ex, "An exception occurred while processing the request.");
        return new NerdPaymentResponse()
        {
            Code = Errors.ServerError,
            Message = $"{OperationErrors[Errors.ServerError]}: {ex.Message}",
            Controls = new Dictionary<string, object>()
        };
    }

    public override async Task<NerdPaymentResponse> FinalAsync(Dictionary<string, object> controls)
    {
        logger.LogInformation("Sending final changes with controls: {controls}.", controls);

        controls["Статус"] = DocumentStatus.COMPLETED.ToString();

        ControlsMessage controlsMessage = new()
        {
            Data = controls
        };

        await sender.SendChangesAsync(controlsMessage);

        logger.LogInformation("FinalAsync completed successfully.");
        return new NerdPaymentResponse()
        {
            Code = Errors.Success,
            Controls = controls,
            Message = OperationErrors[Errors.Success]
        };
    }

    public override async Task<NerdPaymentResponse> PayAsync(string cardNumber, decimal amount, Currency currency, Dictionary<string, object> controls)
    {
        try
        {
            logger.LogInformation("Starting PayAsync for card {cardNumber} with amount {amount} {currency}.", cardNumber, amount, currency);

            PayMoneyRequest payRequest = new(cardNumber, amount, currency);

            var cardIsResponse = await SendCardApiRequestAsync<CheckCardResponse>("https://localhost:7205/microservices/cardis/withdraw", payRequest);

            if (cardIsResponse.Code != (int)Errors.Success)
            {
                logger.LogWarning("CardIs response code {code}: {message}", cardIsResponse.Code, cardIsResponse.Message);
                throw new ArgumentException($"CardIs blocked operation: {cardIsResponse.Code} | {cardIsResponse.Message}");
            }

            controls["Стратегія"] = "CardNerd";
            controls["Сума платежу"] = $"{amount:F2} {currency}";

            controls["Статус"] = DocumentStatus.PAYED.ToString();

            logger.LogInformation("PayAsync completed successfully for card {cardNumber}.", cardNumber);

            return new NerdPaymentResponse()
            {
                Code = Errors.Success,
                Message = OperationErrors[Errors.Success],
                Controls = controls
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during PayAsync.");
            return HandleException(ex);
        }
    }
}
