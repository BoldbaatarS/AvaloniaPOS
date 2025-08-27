using AutoMapper;
using Shared.Models;
using CloudApi.DTOs;

namespace CloudApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // DB model -> DTO
        CreateMap<HallModel, HallDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImagePath));

        // DTO -> DB model (жишээ нь post хийх үед)
        CreateMap<HallDto, HallModel>()
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImageUrl));
    }
}
