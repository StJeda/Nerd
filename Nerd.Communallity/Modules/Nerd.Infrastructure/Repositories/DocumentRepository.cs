using Dapper;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Repositories;

public class DocumentRepository(IDbProvider dbProvider) : IDocumentRepository
{
    public async Task<Document[]> GetDocumentsAsync<Document>()
    {
        string sql = "SELECT * FROM Documents";

        CommandDefinition definition = new(sql);

        IAsyncEnumerable<Document> documents = dbProvider.QueryIncrementallyAsync<Document>(definition);

        return await documents.ToArrayAsync();
    }

    public async Task<int> UpdateDocumentAsync(Guid documentId, string controls, DocumentStatus status)
    {
        string sql = @"UPDATE Documents SET Status = @Status, Controls = @Controls WHERE DocumentId = @DocumentId";

        object pars = new
        {
            DocumentId = documentId,
            Controls = controls,
            Status = status
        };

        int rowsUpdated = await dbProvider.ExecuteAsync(sql, pars);

        return rowsUpdated;
    }

    public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
    {
        var sql = "SELECT * FROM Documents WHERE DocumentId = @DocumentId";

        object pars = new
        {
            DocumentId = documentId
        };

        Document document = await dbProvider.QuerySingleAsync<Document>(sql, pars);

        return document;
    }

    public async Task<int> CreateDocumentAsync(CreateDocumentRequest request, string xmlControls)
    {
        var sql = @"INSERT INTO Documents (
        MerchantName, DocumentId, DebtSeria, Amount, PayerName, PayerCard, Controls
        ) 
        VALUES (
            @MerchantName, @DocumentId, @DebtSeria, @Amount, @PayerName, @PayerCard, @Controls
        );
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await dbProvider.ExecuteAsync(sql, new
        {
            request.MerchantName,
            request.DocumentId,
            request.DebtSeria,
            request.Amount,
            request.PayerName,
            request.PayInfo.PayerCard,
            Controls = xmlControls
        });
    }
}
