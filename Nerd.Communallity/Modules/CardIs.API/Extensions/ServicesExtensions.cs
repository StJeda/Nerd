using FluentValidation;
using MediatR;
using Nerd.Core.Commands;
using Nerd.Core.Handlers.CardChain;
using Nerd.Core.Profiles;
using Nerd.Core.Queries;
using Nerd.Core.Validators;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Infrastructure.DbProvider;
using Nerd.Infrastructure.Handlers;
using Nerd.Infrastructure.Repositories;

namespace Nerd.CardIs.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddCardIsServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddLogging(x => x.AddConsole());

        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<ISender>(provider => provider.GetRequiredService<IMediator>());

        services.AddScoped<IRequestHandler<CheckCardQuery, CheckCardResponse>, CheckCardQueryHandler>();
        services.AddScoped<IRequestHandler<FillCardByCardCommand, PayMoneyResponse>, FillCardByCardCommandHandler>();
        services.AddScoped<IRequestHandler<WithdrawByCardCommand, PayMoneyResponse>, WithdrawByCardCommandHandler>();

        services.AddScoped<ICardIsRepository, CardIsRepository>();
        services.AddScoped<IDbProvider>(provider =>
        {
            string connectionString = builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Get<string>()
                ?? throw new ArgumentNullException("CardIsDb is not provided");
            return new DbProvider(connectionString);
        });

        services.AddScoped<CardCheckHandlerChain>();
        services.AddScoped<CardExistenceHandler>();
        services.AddScoped<CardCheckHandlerChain>();
        services.AddScoped<CardStatusHandler>();
        services.AddScoped<CardPinHandler>();

        services.AddAutoMapper(typeof(СardIsProfile));

        services.AddValidatorsFromAssemblyContaining<CheckCardRequestValidator>();

        services.AddHttpClient();

        return services;
    }
}