using AutoMapper;
using Shared.Models;
using CloudApi.DTOs;

namespace CloudApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Company mapping
        CreateMap<CompanyCreateDto, Company>();
        CreateMap<Company, CompanyDto>();

        // Branch mapping
        CreateMap<Branch, BranchDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));

        CreateMap<BranchCreateDto, Branch>();
        CreateMap<BranchUpdateDto, Branch>();

        // Hall mapping
        CreateMap<HallModel, HallDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImagePath));

        // DTO -> DB model (жишээ нь post хийх үед)
        CreateMap<HallDto, HallModel>()
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImageUrl));

        // Category
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();

        // Product
        CreateMap<Product, ProductDto>();
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

        CreateMap<UserModel, UserDto>()
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name));

        CreateMap<UserCreateDto, UserModel>();
        CreateMap<UserUpdateDto, UserModel>();


       
    }
}
