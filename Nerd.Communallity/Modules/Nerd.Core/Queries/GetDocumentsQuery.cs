using MediatR;
using Nerd.Domain.DTOs;

namespace Nerd.Core.Queries;

public record GetDocumentsQuery() : IRequest<CreateDocumentResponse[]?>;
