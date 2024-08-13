using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Models.AttendeeDto;
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
        CreateMap<AttendeeDetailDto, AttendeeDetail>().ReverseMap();
        CreateMap<RegisterAttendeeDTO, Attendee>()
            .ForMember(dest => dest.AttendeeDetails, opt => opt.MapFrom(src => src.AttendeeDetails)).ReverseMap();
        CreateMap<Attendee, AttendeeDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Phone)).ReverseMap();
    }
}