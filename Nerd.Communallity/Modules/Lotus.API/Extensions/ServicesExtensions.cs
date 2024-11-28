using FluentValidation;
using Lotus.API.Validators;
using MassTransit;
using MediatR;
using Nerd.Core.Commands;
using Nerd.Core.Queries;
using Nerd.Core.Strategies;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Infrastructure.DbProvider;
using Nerd.Infrastructure.Handlers;
using Nerd.Infrastructure.Repositories;
using Nerd.Infrastructure.Senders;

namespace Nerd.Lotus.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddLotusServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddLogging(x => x.AddConsole());

        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<ISender>(p => p.GetRequiredService<IMediator>());

        services.AddScoped<IRequestHandler<GetDocumentsQuery, CreateDocumentResponse[]?>, GetDocumentsQueryHandler>();
        services.AddScoped<IRequestHandler<GetDocumentQuery, CreateDocumentResponse?>, GetDocumentQueryHandler>();
        services.AddScoped<IRequestHandler<CreateDocumentCommand, CreateDocumentResponse>, CreateDocumentCommandHandler>();
        services.AddScoped<IRequestHandler<ReadFromFileQuery, OperationResponse>, ReadFromFileHandler>();

        services.AddScoped<IDebtRepository, DebtRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        services.AddScoped<IDbProvider>(provider =>
        {
            string connectionString = builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Get<string>()
                ?? throw new ArgumentNullException("LotusDb is not provided");
            return new DbProvider(connectionString);
        });

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost:5672");
            });
        });

        services.AddScoped<AbstractPaymentStrategy, CardNerdStrategy>();

        services.AddScoped<IMessageSender, MessageSender>();

        services.AddAutoMapper(typeof(LotusProfile));
        services.AddValidatorsFromAssemblyContaining<CreateDocumentValidator>();

        services.AddHttpClient();

        return services;
    }
}
