using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;

namespace Nerd.Domain.Abstractions;

public interface IDocumentRepository
{
    Task<Document[]> GetDocumentsAsync<Document>();
    Task<int> UpdateDocumentAsync(Guid documentId, string controls, DocumentStatus status);
    Task<Document?> GetDocumentByIdAsync(Guid documentId);
    Task<int> CreateDocumentAsync(CreateDocumentRequest request, string xmlControls);
}