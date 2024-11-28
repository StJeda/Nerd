using Nerd.Domain.Enums;

namespace Nerd.Domain.Models;

public class DebtRecord
{
    public required string DebtSeria { get; set; }
    public required int Id { get; set; }
    public required decimal Amount { get; set; }
    public required string Address { get; set; }
    public required string PostIndex { get; set; }
    public required CommunallityType DebtType { get; set; }
    public string DebtDescription => GetDebtDescription();
    public bool Status { get; set; } = false;
    private string GetDebtDescription()
    {
        switch (DebtType)
        {
            case CommunallityType.WaterDebt:
                return "Debt for Water Supply";
            case CommunallityType.HeatingDebt:
                return "Debt for Heating Services";
            case CommunallityType.ElectricityDebt:
                return "Debt for Electricity Supply";
            case CommunallityType.SquareDebt:
                return "Debt for Property Maintenance";
            case CommunallityType.CleaningDevt:
                return "Debt for Cleaning Services";
            default:
                return "Unknown Debt Type";
        }
    }
}
