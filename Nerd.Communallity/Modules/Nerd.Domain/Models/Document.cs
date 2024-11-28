using Microsoft.AspNetCore.Mvc;
using Nerd.Domain.Enums;
using Nerd.Domain.Utillities;

namespace Nerd.Domain.Models;

public class Document
{
    public Guid DocumentId { get; init; }
    public required string PayerCard { get; set; }   
    public decimal Amount { get; set; }   
    public required string Address { get; set; }    
    public required string PayerName { get; set; } 
    public DocumentStatus Status { get; set; }  
    public required string Controls { get; set; } 
    public required string MerchantName { get; set; }

    [FromQuery]
    public required string Numbers { get; init; }

    [FromQuery]
    public required DateTime ExpireDate { get; init; }

    [FromQuery]
    public char[] PIN { get; init; } = new char[3];
}
