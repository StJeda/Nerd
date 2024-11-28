namespace Nerd.Domain.Models;

public class ControlsMessage
{
    public Dictionary<string, object> Data { get; set; }

    public ControlsMessage()
    {
        Data = new Dictionary<string, object>();
    }
}
