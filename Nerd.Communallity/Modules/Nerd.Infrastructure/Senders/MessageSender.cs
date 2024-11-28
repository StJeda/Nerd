using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nerd.Domain.Abstractions;
using Nerd.Domain.Models;


namespace Nerd.Infrastructure.Senders;

public class MessageSender(ILogger<MessageSender> logger, ISendEndpointProvider sendEndpointProvider) : IMessageSender
{
    public async Task SendChangesAsync(ControlsMessage controls)
    {
        try
        {
            ISendEndpoint endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:eventQueue"));
            await endpoint.Send(controls);

            logger.LogInformation("Changes published to rabbit queue: {Body}", JsonSerializer.Serialize(controls));
        }
        catch (RabbitMqConnectionException ex)
        {
            logger.LogError("CAN'T SEND TO QUEUE: {Details}", ex.Message);
        }
    }
}
