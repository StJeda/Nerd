using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nerd.Core.Commands;
using Nerd.Core.Queries;
using Nerd.Domain.DTOs;
using Nerd.Domain.Utillities;

namespace Nerd.Lotus.API.Endpoints;

public static class EndpointsExtensions
{
    private const string Route = "microservices/lotus/";
    public static IEndpointRouteBuilder AddLotusEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder routeGroup = builder.MapGroup(Route).WithTags("documents");

        routeGroup.MapPost("createDocument", CreateDocument);
        routeGroup.MapGet("getSingleDocument", GetSingleDocument);
        routeGroup.MapGet("getDocuments", GetDocuments);
        routeGroup.MapGet("getGuid", GetGuid);
        routeGroup.MapPost("readDebts", ReadFromPathDebts);

        return builder;
    }

    /// <summary>
    /// Creating a document in Lotus:
    /// </summary>
    /// <returns><see cref="CreateDocumentResponse"/> model</returns>
    /// <remarks>
    /// Sample request 
    ///
    ///     {
    ///         "documentId": "a1b2c3d4-e5f6-7890-1234-56789abcdef0",
    ///         "payerName": "Yarmolenko M.O",
    ///         "debtSeria": "54323344",
    ///         "amount": 100,
    ///         "merchantName": "NerdMerchant"
    ///     }
    /// 
    ///</remarks>
    ///
    private static async Task<CreateDocumentResponse> CreateDocument([FromBody] CreateDocumentCommand command, [FromQuery] string payerCard, [FromQuery] string PIN, [FromQuery] DateTime yyMM, [FromServices] ISender sender)
    {
        CardRequest cardRequest = new(payerCard, yyMM, PIN);
        
        command.PayInfo = cardRequest;

        return await sender.Send(command);
    }

    private static async Task<CreateDocumentResponse?> GetSingleDocument([FromQuery] Guid documentId, [FromServices] ISender sender) => await sender.Send(new GetDocumentQuery(documentId));

    private static async Task<CreateDocumentResponse[]?> GetDocuments([FromServices] ISender sender) => await sender.Send(new GetDocumentsQuery());

    private static Guid GetGuid() => GuidUtility.GenerateSemiGuid();

    private static async Task<OperationResponse> ReadFromPathDebts([FromQuery] string path, [FromServices] ISender sender) => await sender.Send(new ReadFromFileQuery(path));
}