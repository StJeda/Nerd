using Nerd.Domain.Models;

namespace Nerd.Domain.DTOs;

public record OperationResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<DebtRecord>? DebtRecords { get; set; }
}
