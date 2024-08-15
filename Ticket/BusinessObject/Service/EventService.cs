using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.Service
{
    public class EventService : IEventService
    {
        private readonly IUserRepo _userRepo;
        private readonly IEventRepo _eventRepo;
        private readonly ITicketRepo _ticketRepo;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly Cloudinary _cloudinary;

        public EventService(IEventRepo repo, AppConfiguration configuration, IMapper mapper, Cloudinary cloudinary,
            IUserRepo userRepo, ITicketRepo ticketRepo)
        {
            _eventRepo = repo;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _userRepo = userRepo;
            _ticketRepo = ticketRepo;
            _appConfiguration = configuration;
        }

        public async Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetAllEvents(int page, int pageSize,
            string search, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewEventDTO>>();
            try
            {
                var events = await _eventRepo.GetEvent();
                if (!string.IsNullOrEmpty(search))
                {
                    events = events.Where(e => (e.Title.Contains(search, StringComparison.OrdinalIgnoreCase)));
                    ;
                }

                events = sort.ToLower().Trim() switch
                {
                    "title" => events.OrderBy(e => e?.Title),
                    "startdate" => events.OrderBy(e => e?.StartDate),
                    "enddate" => events.OrderBy(e => e?.EndDate),
                    _ => events.OrderBy(e => e.Id).ToList()
                };

                var map = _mapper.Map<IEnumerable<ViewEventDTO>>(events);
                var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                res.Data = paging;
                res.Success = true;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to get Event: {e.Message}";
            }

            return res;
        }

        public async Task<ServiceResponse<ViewEventDTO>> GetEventById(int id)
        {
            var res = new ServiceResponse<ViewEventDTO>();
            try
            {
                var eventEntity = await _eventRepo.GetEventById(id);
                if (eventEntity != null)
                {
                    var mappedEvent = _mapper.Map<ViewEventDTO>(eventEntity);
                    res.Data = mappedEvent;
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Event not found";
                }
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Error retrieving event: {ex.Message}";
            }

            return res;
        }

        public async Task<ServiceResponse<ViewEventDTO>> CreateEvent(CreateEventDTO eventDTO)
        {
            var result = new ServiceResponse<ViewEventDTO>();
            try
            {
                if (!string.IsNullOrEmpty(eventDTO.Title))
                {
                    // Check if the event title already exists
                    var eventExist = await _eventRepo.CheckExistByTitle(eventDTO.Title);
                    if (eventExist != null)
                    {
                        result.Success = false;
                        result.Message = "Event with the same name already exists!";
                        return result;
                    }

                    // Check if the organizer exists
                    var organizerExist = await _userRepo.GetByIdAsync(eventDTO.OrganizerId);
                    if (organizerExist == null)
                    {
                        result.Success = false;
                        result.Message = "Organizer not found!";
                        return result;
                    }

                    // Upload image if provided
                    var imageUrl = await UploadImageCollection(eventDTO.ImageUrl);

                    // Create the event entity
                    var Event = new Event
                    {
                        Title = eventDTO.Title,
                        ImageUrl = imageUrl,
                        StartDate = DateOnly.FromDateTime(eventDTO.StartDate),
                        EndDate = DateOnly.FromDateTime(eventDTO.EndDate),
                        OrganizerId = eventDTO.OrganizerId,
                        VenueId = eventDTO.VenueId,
                        Description = eventDTO.Description,
                        Status = "Pending"
                    };

                    // Save the event to the database
                    await _eventRepo.AddAsync(Event);

                    // Map to DTO
                    var res = _mapper.Map<ViewEventDTO>(Event);

                    result.Data = res;
                    result.Success = true;
                    result.Message = "Event created successfully!";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.InnerException != null
                    ? e.InnerException.Message + "\n" + e.StackTrace
                    : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<string> UploadImageCollection(IFormFile file)
        {
            if (file.Length <= 0) throw new Exception("Failed to upload image");
            await using var stream = file.OpenReadStream();
            var upLoadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream),
                Transformation = new Transformation().Crop("fill").Gravity("face")
            };
            var uploadResult = await _cloudinary.UploadAsync(upLoadParams);

            if (uploadResult.Url != null)
            {
                return uploadResult.Url.ToString();
            }

            throw new Exception("Failed to upload image");
        }


        public async Task<ServiceResponse<bool>> DeleteEvent(int id)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                await _eventRepo.DeleteEvent(id);
                res.Success = true;
                res.Message = "Event deleted successfully.";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Failed to delete event: {ex.Message}";
            }

            return res;
        }


        public async Task<ServiceResponse<ViewEventDTO>> UpdateEvent(int id, UpdateEventDTO eventDTO)
        {
            var res = new ServiceResponse<ViewEventDTO>();
            try
            {
                var eventToUpdate = await _eventRepo.GetEventById(id);
                if (eventToUpdate == null)
                {
                    res.Success = false;
                    res.Message = "Event not found!";
                    return res;
                }

                // Update the fields that can be changed
                eventToUpdate.Title = eventDTO.Title;
                eventToUpdate.Description = eventDTO.Description;
                eventToUpdate.VenueId = eventDTO.VenueId;
                eventToUpdate.StartDate = DateOnly.FromDateTime(eventDTO.StartDate);
                eventToUpdate.EndDate = DateOnly.FromDateTime(eventDTO.EndDate);

                // If a new image is provided, upload it
                if (eventDTO.ImageFile != null)
                {
                    var imageUrl = await UploadImageCollection(eventDTO.ImageFile);
                    eventToUpdate.ImageUrl = imageUrl;
                }

                await _eventRepo.UpdateEvent(id, eventToUpdate);

                var result = _mapper.Map<ViewEventDTO>(eventToUpdate);
                res.Success = true;
                res.Message = "Event updated successfully";
                res.Data = result;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Failed to update event: {ex.Message}";
            }

            return res;
        }

        public async Task<ServiceResponse<bool>> ChangeEventStatus(ChangeEventStatusDTO statusDTO)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var eventToUpdate = await _eventRepo.GetEventById(statusDTO.EventId);
                if (eventToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "Event not found.";
                    return response;
                }

                // Only allow changing to "Active" from "Pending"
                if (eventToUpdate.Status != "Pending" || statusDTO.Status != "Active")
                {
                    response.Success = false;
                    response.Message = "Invalid status change.";
                    return response;
                }

                eventToUpdate.Status = statusDTO.Status;
                await _eventRepo.UpdateEvent(eventToUpdate.Id, eventToUpdate);

                response.Success = true;
                response.Message = "Event status updated successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CreateEventWithTicketsDTO>> CreateEventWithTickets(
            CreateEventWithTicketsDTO dto)
        {
            var result = new ServiceResponse<CreateEventWithTicketsDTO>();
            try
            {
                // Create event
                var eventEntity = new Event
                {
                    Title = dto.Title,
                    ImageUrl = await UploadImageCollection(dto.ImageUrl),
                    StartDate = DateOnly.FromDateTime(dto.StartDate),
                    EndDate = DateOnly.FromDateTime(dto.EndDate),
                    OrganizerId = dto.OrganizerId,
                    VenueId = dto.VenueId,
                    Description = dto.Description,
                    Status = "Pending"
                };

                await _eventRepo.AddAsync(eventEntity);

                // Create tickets
                var ticket = new Ticket
                {
                    EventId = eventEntity.Id,
                    Price = dto.Ticket.Price,
                    Quantity = dto.Ticket.Quantity, 
                    TicketSaleEndDate = dto.Ticket.TicketSaleEndDate
                };
                await _ticketRepo.AddAsync(ticket);

                result.Data = dto;
                result.Success = true;
                result.Message = "Event and tickets created successfully!";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}