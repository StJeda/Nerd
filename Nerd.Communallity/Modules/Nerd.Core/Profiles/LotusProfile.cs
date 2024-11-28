using AutoMapper;
using Nerd.Core.Commands;
using Nerd.Core.Extensions;
using Nerd.Domain.DTOs;
using Nerd.Domain.Models;

public class LotusProfile : Profile
{
    public LotusProfile()
    {
        CreateMap<CreateDocumentCommand, CreateDocumentRequest>();

        CreateMap<Document, CreateDocumentResponse>()
            .ConvertUsing(x => new CreateDocumentResponse
            {
                DocumentId = x.DocumentId,
                Status = x.Status,
                Controls = x.Controls.DeserializeXmlToDictionary()
            });
    }
}
