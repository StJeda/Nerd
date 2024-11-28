using System.Globalization;
using OfficeOpenXml;
using MediatR;
using Nerd.Domain.Abstractions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using Nerd.Domain.Models;


namespace Nerd.Infrastructure.Handlers;

public record ReadFromFileHandler(IDebtRepository repository) : IRequestHandler<ReadFromFileQuery, OperationResponse>
{
    public async Task<OperationResponse> Handle(ReadFromFileQuery request, CancellationToken cancellationToken)
    {
        var response = new OperationResponse
        {
            Message = string.Empty,
            Success = true,
            DebtRecords = new List<DebtRecord>()
        };

        try
        {
            if (File.Exists(request.path) is false)
            {
                response.Success = false;
                response.Message = "File not found.";
                return response;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
			using ExcelPackage package = new(new FileInfo(request.path));
            
			ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            
			int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                DebtRecord debtRecord = new()
                {
                    DebtSeria = worksheet.Cells[row, 1].Text.Trim(),
                    Id = int.Parse(worksheet.Cells[row, 2].Text.Trim()),
                    Amount =  decimal.Parse(worksheet.Cells[row, 3].Text, CultureInfo.InvariantCulture),
                    Address = worksheet.Cells[row, 4].Text.Trim(),
                    PostIndex = worksheet.Cells[row, 5].Text.Trim(),
                    DebtType = ParseDebtType(worksheet.Cells[row, 6].Text.Trim())
                };

				string debtTypeText = worksheet.Cells[row, 6].Text.Trim();
                
				debtRecord.DebtType = ParseDebtType(debtTypeText);

                response.DebtRecords.Add(debtRecord);
            }

            for(int i = 0; i < response.DebtRecords.Count(); i++)            
                await repository.CreateDebtRecordAsync(response.DebtRecords[i]);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error reading Excel file: {ex.Message}";
        }

        return response;
    }

    private CommunallityType ParseDebtType(string debtTypeText)
    {
        switch (debtTypeText.ToLower())
        {
            case "water":
                return CommunallityType.WaterDebt;
            case "heating":
                return CommunallityType.HeatingDebt;
            case "electricity":
                return CommunallityType.ElectricityDebt;
            case "square":
                return CommunallityType.SquareDebt;
            case "cleaning":
                return CommunallityType.CleaningDevt;
            default:
                return CommunallityType.Unknown;
        }
    }
}
