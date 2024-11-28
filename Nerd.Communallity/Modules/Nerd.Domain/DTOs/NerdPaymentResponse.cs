using Nerd.Domain.Enums;

namespace Nerd.Domain.DTOs;

public record NerdPaymentResponse
{
    public required string Message { get; init; }
    public required Errors Code { get; init; }
    public required Dictionary<string, object> Controls { get; init; }
}
