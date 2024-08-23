using AutoMapper;
using BusinessObject.Models.AttendeeDto;
using BusinessObject.Models.BoothDTO;
using BusinessObject.Models.BoothRequestDTO;
using BusinessObject.Models.EventDTO;
using BusinessObject.Models.GiftDTO;
using BusinessObject.Models.GiftReceptionDTO;
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
        CreateMap<UpdateAttendeeDto, AttendeeDetail>().ReverseMap();
        CreateMap<RegisterAttendeeDTO, Attendee>()
            .ForMember(dest => dest.CheckInCode, opt => opt.Ignore()) // Ignore CheckInCode
            .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate))
            .ForMember(dest => dest.AttendeeDetails, opt => opt.MapFrom(src => src.AttendeeDetails))
            .ReverseMap();
        CreateMap<Attendee, AttendeeDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Phone))
            .ReverseMap();
        CreateMap<Attendee, AttendeeDetailDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.AttendeeDetails.FirstOrDefault()!.Phone))
            .ForMember(dest => dest.CheckInCode, opt => opt.MapFrom(src => src.CheckInCode))
            .ForMember(dest => dest.CheckInStatus, opt => opt.MapFrom(src => src.CheckInStatus))
            .ReverseMap();

        CreateMap<AttendeeDetail, AttendeeDetailDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ReverseMap();
        CreateMap<AttendeeDetailRegisterDto, AttendeeDetail>().ReverseMap();

        CreateMap<CreateEventDTO, Event>()
            .ReverseMap();

        CreateMap<Event, ViewEventDTO>()
            .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageUrl))
            .ReverseMap();
        CreateMap<UpdateEventDTO, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ReverseMap(); // Image will be handled separately.ReverseMap();
        CreateMap<Event, ViewOrganizerEventDTO>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Ticket, opt => opt.MapFrom(src => src.Tickets.FirstOrDefault()))
            .ReverseMap();
        CreateMap<Ticket, EventTicket>()
            .ReverseMap();

        CreateMap<Venue, CreateVenueDTO>().ReverseMap();
        CreateMap<Venue, ViewVenueDTO>().ReverseMap();
        CreateMap<VenueStatusDTO, Venue>().ReverseMap();

        CreateMap<CreateTicketDTO, Ticket>().ReverseMap();
        CreateMap<ViewTicketDTO, Ticket>().ReverseMap();

        CreateMap<CreateBoothDTO, Booth>().ReverseMap();
        CreateMap<ViewBoothDTO, Booth>().ReverseMap();
        CreateMap<BoothStatusDTO, Booth>().ReverseMap();

        CreateMap<CreateBoothRequestDTO, BoothRequest>().ReverseMap();
        CreateMap<BoothRequest, ViewBoothRequestDTO>()
            .ForMember(dest => dest.BoothName, opt => opt.MapFrom(src => src.Booth.Name))
            .ForMember(dest => dest.SponsorName, opt => opt.MapFrom(src => src.Sponsor.Name))
            .ReverseMap();
        CreateMap<BoothRequestStatusDTO, BoothRequest>().ReverseMap();

        CreateMap<Transaction, CreateTransactionDto>().ReverseMap();
        CreateMap<Transaction, TransactionDto>().ReverseMap();

        CreateMap<Payment, CreatePaymentMethodDto>().ReverseMap();
        CreateMap<Payment, PaymentMethodDto>().ReverseMap();

        CreateMap<CreateGiftDTO, Gift>().ReverseMap();
        CreateMap<Gift, ViewGiftDTO>()
            .ForMember(dest => dest.BoothName, opt => opt.MapFrom(src => src.Booth.Name))
            .ReverseMap();

        CreateMap<CreateGiftReceptionDTO, GiftReception>().ReverseMap();
        CreateMap<GiftReception, ViewGiftReceptionDTO>()
            .ForMember(dest => dest.GiftName, opt => opt.MapFrom(src => src.Gift.Name))
            .ReverseMap();
    }
}