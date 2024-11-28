using MassTransit;
using Microsoft.Extensions.Logging;
using Nerd.Domain.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Nerd.DocumentWorker.Consumers;

public class ControlsConsumer(ILogger<ControlsConsumer> logger) : IConsumer<ControlsMessage>
{
    public Task Consume(ConsumeContext<ControlsMessage> context)
    {
        var messageData = context.Message;
        string pdfFilePath = Path.Combine("C:\\Users\\kagucuti\\source\\repos\\Nerd\\Docs\\kv", $"Communal_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        CreatePdfReceipt(messageData.Data, pdfFilePath);
        logger.LogInformation($"[INFO] kv save in {pdfFilePath}.");
        
        return Task.CompletedTask;
    }

    private void CreatePdfReceipt(Dictionary<string, object> data, string filePath)
    {
        PdfDocument document = new PdfDocument();
        document.Info.Title = $"Комунальна квитанція #{data["debtSeria"]}";

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        XFont headerFont = new XFont("Arial", 14, XFontStyle.Regular);
        XFont regularFont = new XFont("Arial", 14, XFontStyle.Regular);
        double margin = 40;
        double yPosition = margin;

        DrawHeader(gfx, ref yPosition, page, headerFont);

        XFont font = regularFont;
        foreach (var entry in data)
        {
            string line = $"{entry.Key}: {entry.Value}";
            gfx.DrawString(line, font, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, page.Height), XStringFormats.TopLeft);
            yPosition += 20;

            if (yPosition > page.Height - margin)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                yPosition = margin;
                DrawHeader(gfx, ref yPosition, page, headerFont);
            }
        }

        document.Save(filePath);
    }

    private void DrawHeader(XGraphics gfx, ref double yPosition, PdfPage page, XFont headerFont)
    {
        string title = "Комунальна квитанція";
        gfx.DrawString(title, headerFont, XBrushes.Black, new XRect(40, yPosition, page.Width - 80, 40), XStringFormats.TopCenter);
        yPosition += 40;

        XFont subHeaderFont = new XFont("Arial", 14, XFontStyle.Regular);
        string companyName = "Lotus - Комунальне підприємство";
        string companyAddress = "Адреса: вул. Вишнева, 45, Київ, Україна";
        string companyPhone = "Телефон: +380 44 123 4567";

        gfx.DrawString(companyName, subHeaderFont, XBrushes.Black, new XRect(40, yPosition, page.Width - 80, 40), XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString(companyAddress, subHeaderFont, XBrushes.Black, new XRect(40, yPosition, page.Width - 80, 40), XStringFormats.TopLeft);
        yPosition += 20;
        gfx.DrawString(companyPhone, subHeaderFont, XBrushes.Black, new XRect(40, yPosition, page.Width - 80, 40), XStringFormats.TopLeft);
        yPosition += 30;
        gfx.DrawLine(XPens.Red, new XPoint(5, yPosition), new XPoint(page.Width - 5, yPosition));
        yPosition += 10;
    }


}
