using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nerd.Core.Commands;
using Nerd.Core.Queries;
using Nerd.Domain.DTOs;

namespace Nerd.CardIs.API.Endpoints;

public static class EndpointsExtensions
{
    private const string Route = "microservices/cardis/";

    public static IEndpointRouteBuilder AddCardIsEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder routeGroup = builder.MapGroup(Route).WithTags("cards");

        routeGroup.MapPost("checkCard", CheckCardAsync);
        routeGroup.MapPost("fillCard", FillCardAsync);
        routeGroup.MapPost("withdraw", WithdrawByCardAsync);

        return builder;
    }

    private static async Task<CheckCardResponse> CheckCardAsync([FromBody] CheckCardQuery query, ISender sender) => await sender.Send(query);

    private static async Task<PayMoneyResponse> FillCardAsync([FromBody] FillCardByCardCommand command, ISender sender) => await sender.Send(command);

    private static async Task<PayMoneyResponse> WithdrawByCardAsync([FromBody] WithdrawByCardCommand command, ISender sender) => await sender.Send(command);
}