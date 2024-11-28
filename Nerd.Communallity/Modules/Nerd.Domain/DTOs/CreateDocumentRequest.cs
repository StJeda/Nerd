using Nerd.Domain.Enums;
using Nerd.Domain.Utillities;
using System.Text.Json.Serialization;

namespace Nerd.Domain.DTOs;

public record CreateDocumentRequest
{
    public required Guid DocumentId { get; init; } = GuidUtility.GenerateSemiGuid();
    public Dictionary<string, object> Controls { get; set; } = [];

    [JsonIgnore]
    public CardRequest PayInfo { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currency Currency { get; init; } = Currency.UAH;

    public required string PayerName { get; init; }
    public required decimal Amount { get; init; }
    public required string MerchantName { get; init; }
    public required string DebtSeria { get; init; }
}