using Dapper;
using Nerd.Domain.Abstractions;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Repositories
{
    public class DebtRepository(IDbProvider dbProvider) : IDebtRepository
    {

        public async Task<DebtRecord[]> GetDebtRecordsAsync()
        {
            string sql = "SELECT * FROM DebtRecords";

            CommandDefinition definition = new(sql);

            IAsyncEnumerable<DebtRecord> debtRecords = dbProvider.QueryIncrementallyAsync<DebtRecord>(definition);

            return await debtRecords.ToArrayAsync();
        }

        public async Task<DebtRecord?> GetDebtRecordByIdAsync(string debtSeria)
        {
            string sql = "SELECT * FROM DebtRecords WHERE DebtSeria = @DebtSeria";

            object pars = new
            {
                DebtSeria = debtSeria
            };

            DebtRecord debtRecord = await dbProvider.QuerySingleAsync<DebtRecord>(sql, pars);

            return debtRecord;
        }

        public async Task<int> CreateDebtRecordAsync(DebtRecord debtRecord)
        {
            string sql = @"
            INSERT INTO DebtRecords (
                DebtSeria, Id, Amount, Address, PostIndex, DebtType, Status
            ) 
            VALUES (
                @DebtSeria, @Id, @Amount, @Address, @PostIndex, @DebtType, @Status
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var pars = new
            {
                debtRecord.DebtSeria,
                debtRecord.Id,
                debtRecord.Amount,
                debtRecord.Address,
                debtRecord.PostIndex,
                debtRecord.DebtType,
                debtRecord.Status
            };

            return await dbProvider.ExecuteAsync(sql, pars);
        }

        public async Task<int> UpdateDebtRecordStatusAsync(string debtSeria, bool status)
        {
            string sql = @"UPDATE DebtRecords 
                           SET Status = @Status 
                           WHERE DebtSeria = @DebtSeria";

            var pars = new
            {
                DebtSeria = debtSeria,
                Status = status
            };

            int rowsUpdated = await dbProvider.ExecuteAsync(sql, pars);

            return rowsUpdated;
        }
    }
}