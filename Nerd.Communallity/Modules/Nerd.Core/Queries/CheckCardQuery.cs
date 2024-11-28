using MediatR;
using Nerd.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Nerd.Core.Queries;

public record CheckCardQuery([property: Required] string PayerCard, [property: Required] string PIN, [property: Required] DateTime expDate) : IRequest<CheckCardResponse>;
