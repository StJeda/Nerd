using MediatR;
using Nerd.Domain.DTOs;

namespace Nerd.Core.Queries;

public record GetDocumentQuery(Guid DocumentId) : IRequest<CreateDocumentResponse?>;
