using Nerd.Domain.Utillities;

namespace Nerd.Domain.Models;

public class Card
{
    public Guid CardId { get; set; } = GuidUtility.GenerateSemiGuid();
    public string Numbers { get; set; } = null!;
    public decimal Balance { get; set; } = 0.0m;
    public DateTime ExpireDate { get; set; } = DateTime.Now.AddYears(4);
    public required string PIN { get; set; }
    public bool Status { get; set; } = true;
}