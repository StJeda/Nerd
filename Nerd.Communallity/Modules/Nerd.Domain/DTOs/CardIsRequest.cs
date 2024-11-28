using System.ComponentModel.DataAnnotations;

namespace Nerd.Domain.DTOs;

public record CheckCardRequest([property: Required] string PayerCard, [property: Required] string PIN, [property: Required] DateTime expDate);