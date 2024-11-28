using Microsoft.AspNetCore.Mvc;
using Nerd.Domain.DTOs;

namespace Nerd.Domain.Abstractions;

public interface ILotusService
{
    OperationResponse ReadFromPath(string path);
    Task<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request, string payerCard, string PIN, DateTime yyMM);
    Task<CreateDocumentResponse?> GetDocumentAsync(Guid DocumentId);
    Task<CreateDocumentResponse[]?> GetDocumentsAsync();
}