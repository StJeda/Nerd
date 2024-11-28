using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Nerd.Core.Extensions;
using Nerd.Core.Queries;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Handlers;

public record GetDocumentQueryHandler(IDocumentRepository repository,
    IMapper mapper,
    ILogger<GetDocumentQueryHandler> logger) : IRequestHandler<GetDocumentQuery, CreateDocumentResponse?>
{
    public async Task<CreateDocumentResponse?> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.DocumentId == Guid.Empty)
            {
                throw new ArgumentNullException("DocumentId must be not null");
            }

            Document document = await repository.GetDocumentByIdAsync(request.DocumentId) ?? throw new NullReferenceException("Document must be not null");
            return mapper.Map<CreateDocumentResponse>(document);
        }
        catch (Exception ex)
        {
            logger.LogErrorEvent(ex.Message);
            return null;
        }
    }
}
