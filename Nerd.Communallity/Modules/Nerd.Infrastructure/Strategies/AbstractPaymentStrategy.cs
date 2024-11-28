using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;

namespace Nerd.Domain.Abstractions;

public abstract class AbstractPaymentStrategy
{
    public abstract Task<NerdPaymentResponse> ExecuteAsync(NerdPaymentRequest request, Dictionary<string, object> controls);
    public abstract Task<NerdPaymentResponse> ConfirmAsync(string cardNumber, string PIN, DateTime expDate, Dictionary<string, object> controls);
    public abstract Task<NerdPaymentResponse> PayAsync(string cardNumber, decimal amount, Currency currency, Dictionary<string, object> controls);
    public abstract Task<NerdPaymentResponse> FinalAsync(Dictionary<string, object> controls);
}
