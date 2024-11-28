using System.ComponentModel.DataAnnotations;

namespace Nerd.Domain.DTOs;

public record CardRequest([property: Required] string PayerCard, [property: Required] DateTime expDate, [property: Required] string PIN);
