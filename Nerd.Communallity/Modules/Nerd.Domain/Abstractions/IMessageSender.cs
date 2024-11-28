using Nerd.Domain.Models;

namespace Nerd.Domain.Abstractions;

public interface IMessageSender
{
    Task SendChangesAsync(ControlsMessage controls);
}