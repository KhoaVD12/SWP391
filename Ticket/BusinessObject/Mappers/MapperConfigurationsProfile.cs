using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Models.BoothDTO;
using BusinessObject.Models.BoothRequestDTO;
using BusinessObject.Models.EventDTO;
using BusinessObject.Models.TicketDTO;
using BusinessObject.Models.UserDTO;
using BusinessObject.Models.VenueDTO;
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

        CreateMap<CreateEventDTO, Event>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndDate)))
            .ReverseMap() // Automatically maps from Event to CreateEventDTO
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)));
        CreateMap<ViewEventDTO, Event>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndDate)))
            .ReverseMap() // Automatically maps from Event to CreateEventDTO
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)));

        CreateMap<Venue, CreateVenueDTO>().ReverseMap();

        CreateMap<CreateTicketDTO, Ticket>().ReverseMap();
        
        CreateMap<CreateBoothDTO, Booth>().ReverseMap();

        CreateMap<CreateBoothRequestDTO, BoothRequest>().ReverseMap();
    }
}