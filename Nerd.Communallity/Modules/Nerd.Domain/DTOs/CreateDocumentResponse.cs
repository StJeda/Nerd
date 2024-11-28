using Nerd.Domain.Enums;
using System.Text.Json.Serialization;

namespace Nerd.Domain.DTOs;

public class CreateDocumentResponse
{
    public Guid? DocumentId { get; init; }
    public string? OperationCode { get; init; } = Errors.Success.ToString();
    public Dictionary<string, object> Controls { get; set; } = [];
    public string? OperationMessage { get; init; } = "Success!";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required DocumentStatus Status { get; set; } = DocumentStatus.COMPLETED;
}