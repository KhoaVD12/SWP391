using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using DataAccessObject.Entities;

namespace BusinessObject.Mappers;

public class MapperConfigurationsProfile : Profile
{
    public MapperConfigurationsProfile()
    {
        CreateMap<User, LoginResquestDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<ResetPassDTO, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
        CreateMap<User, ResetPassDTO>();
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<User, UserUpdateDTO>().ReverseMap();
        CreateMap<User, UserStatusDTO>().ReverseMap();
    }
}