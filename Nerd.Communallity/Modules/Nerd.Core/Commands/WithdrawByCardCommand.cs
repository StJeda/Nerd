using MediatR;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nerd.Core.Commands;

public record WithdrawByCardCommand([property: Required, JsonPropertyName("payercard")] string PayerCard, [property: Required, JsonPropertyName("amount")] decimal Amount, [property: Required, JsonPropertyName("currency")] Currency Currency) : IRequest<PayMoneyResponse>;