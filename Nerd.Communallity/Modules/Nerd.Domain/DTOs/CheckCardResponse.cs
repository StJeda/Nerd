using Nerd.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nerd.Domain.DTOs;

public record CheckCardResponse([property: Required, JsonPropertyName("message")] string Message, [property: Required, JsonPropertyName("code")] Errors Code);