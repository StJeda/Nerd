using Nerd.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nerd.Domain.DTOs;

public record PayMoneyRequest([property: Required, JsonPropertyName("payercard")] string PayerCard, [property: Required, JsonPropertyName("amount")] decimal Amount, [property: Required, JsonPropertyName("currency")] Currency Currency);
