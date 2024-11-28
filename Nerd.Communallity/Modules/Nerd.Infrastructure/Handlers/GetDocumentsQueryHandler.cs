using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Nerd.Core.Extensions;
using Nerd.Core.Queries;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Handlers;

public record GetDocumentsQueryHandler(IDocumentRepository repository, IMapper mapper, ILogger<GetDocumentsQueryHandler> logger) : IRequestHandler<GetDocumentsQuery, CreateDocumentResponse[]?>
{
    public async Task<CreateDocumentResponse[]?> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Document[] documents = await repository.GetDocumentsAsync<Document>();
            return mapper.Map<CreateDocumentResponse[]>(documents);
        }
        catch (Exception ex)
        {
            logger.LogErrorEvent(ex.Message);
            return null;
        }
    }
}

