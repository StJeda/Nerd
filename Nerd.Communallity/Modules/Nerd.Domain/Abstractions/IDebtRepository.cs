using Nerd.Domain.Models;

namespace Nerd.Domain.Abstractions;

public interface IDebtRepository
{
    Task<DebtRecord[]> GetDebtRecordsAsync(); 
    Task<DebtRecord?> GetDebtRecordByIdAsync(string debtSeria);
    Task<int> CreateDebtRecordAsync(DebtRecord debtRecord);
    Task<int> UpdateDebtRecordStatusAsync(string debtSeria, bool status);
}
