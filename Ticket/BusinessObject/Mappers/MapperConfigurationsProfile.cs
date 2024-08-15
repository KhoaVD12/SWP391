using AutoMapper;
using BusinessObject.Models;
using BusinessObject.Models.AttendeeDto;
using BusinessObject.Models.BoothDTO;
using BusinessObject.Models.BoothRequestDTO;
using BusinessObject.Models.EventDTO;
using BusinessObject.Models.GiftDTO;
using BusinessObject.Models.PaymentDTO;
using BusinessObject.Models.TicketDTO;
using BusinessObject.Models.TransactionDTO;
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
        CreateMap<AttendeeDetailDto, AttendeeDetail>().ReverseMap();
        CreateMap<RegisterAttendeeDTO, Attendee>()
            .ForMember(dest => dest.AttendeeDetails, opt => opt.MapFrom(src => src.AttendeeDetails)).ReverseMap();
        CreateMap<Attendee, AttendeeDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Phone))
            .ReverseMap();

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
        CreateMap<Venue, ViewVenueDTO>().ReverseMap();

        CreateMap<CreateTicketDTO, Ticket>().ReverseMap();
        CreateMap<ViewTicketDTO, Ticket>().ReverseMap();

        CreateMap<CreateBoothDTO, Booth>().ReverseMap();
        CreateMap<ViewBoothDTO, Booth>().ReverseMap();

        CreateMap<CreateBoothRequestDTO, BoothRequest>().ReverseMap();
        CreateMap<ViewBoothRequestDTO, BoothRequest>().ReverseMap();

        CreateMap<Transaction, CreateTransactionDto>().ReverseMap();
        CreateMap<Transaction, TransactionDto>().ReverseMap();

        CreateMap<Payment, CreatePaymentMethodDto>().ReverseMap();
        CreateMap<Payment, PaymentMethodDto>().ReverseMap();

        CreateMap<CreateGiftDTO, Gift>().ReverseMap();
        CreateMap<ViewGiftDTO, Gift>().ReverseMap();
    }
}