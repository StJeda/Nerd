using System.Transactions;
using Nerd.Domain.Abstractions;
using Nerd.Domain.Models;

namespace Nerd.Infrastructure.Repositories;

public class CardIsRepository(IDbProvider dbProvider) : ICardIsRepository
{
    public async Task<Card?> GetCardAsync(string numbers)
    {
        var sql = "SELECT * FROM Cards WHERE Numbers = @Numbers";

        object pars = new
        {
            Numbers = numbers,
        };

        return await dbProvider.QuerySingleAsync<Card>(sql, pars);
    }

    public async Task<int> TransferFundsAsync(string sourcePayerCard, string destinationPayerCard, decimal amount)
    {
        string checkSourceCardSql = @"SELECT COUNT(1) FROM Cards WHERE Numbers = @Numbers";

        int sourceCardExists = await dbProvider.ExecuteScalarAsync<int>(checkSourceCardSql, new { PayerCard = sourcePayerCard });

        if (sourceCardExists == 0)
        {
            throw new InvalidOperationException($"Card {sourcePayerCard} is not found.");
        }

        string checkDestinationCardSql = @"SELECT COUNT(1) FROM Cards WHERE Numbers = @Numbers";

        int destinationCardExists = await dbProvider.ExecuteScalarAsync<int>(checkDestinationCardSql, new { PayerCard = destinationPayerCard });

        if (destinationCardExists == 0)
        {
            throw new InvalidOperationException($"Card {destinationPayerCard} is not found.");
        }

        string checkBalanceSql = @"SELECT Balance FROM Cards WHERE Numbers = @Numbers";

        decimal currentBalance = await dbProvider.QuerySingleAsync<decimal>(checkBalanceSql, new { PayerCard = sourcePayerCard });

        if (currentBalance < amount)
        {
            throw new InvalidOperationException($"\"A low balance {sourcePayerCard}.  Balance: {currentBalance}, amount of withdraw: {amount}");
        }

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        string updateSourceBalanceSql = @"UPDATE Cards SET Balance = Balance - @Amount WHERE Numbers = @Numbers";

        object sourceParams = new 
        { 
            Numbers = sourcePayerCard,
            Amount = amount 
        };
        int sourceRowsUpdated = await dbProvider.ExecuteAsync(updateSourceBalanceSql, sourceParams);

        string updateBalanceSql = @"UPDATE Cards SET Balance = Balance + @Amount WHERE Numbers = @Numbers";
        
        object destinationParams = new 
        { 
            Numbers = destinationPayerCard,
            Amount = amount 
        };

        int destinationRowsUpdated = await dbProvider.ExecuteAsync(updateBalanceSql, destinationParams);

        transactionScope.Complete();

        return sourceRowsUpdated + destinationRowsUpdated;
    }

    public async Task<int> WithdrawFromCardAsync(string numbers, decimal amount)
    {
        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        string checkBalanceSql = @"SELECT Balance FROM Cards WHERE Numbers = @Numbers";

        decimal currentBalance = await dbProvider.QuerySingleAsync<decimal>(checkBalanceSql, new { Numbers = numbers });

        if (currentBalance < amount)
        {
            throw new InvalidOperationException($"A low balance {numbers}. Balance: {currentBalance}, amount of withdraw: {amount}");
        }

        string updateBalanceSql = @"UPDATE Cards SET Balance = Balance - @Amount WHERE Numbers = @Numbers";

        var parameters = new { Numbers = numbers, Amount = amount };

        int rowsUpdated = await dbProvider.ExecuteAsync(updateBalanceSql, parameters);

        transactionScope.Complete();

        return rowsUpdated;
    }
}