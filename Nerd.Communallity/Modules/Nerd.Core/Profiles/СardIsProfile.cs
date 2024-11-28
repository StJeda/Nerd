using AutoMapper;
using Nerd.Core.Queries;
using Nerd.Domain.DTOs;

namespace Nerd.Core.Profiles;

public class СardIsProfile : Profile
{
    public СardIsProfile()
    {
        CreateMap<CheckCardQuery, CheckCardRequest>();
    }
}
