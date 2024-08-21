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
using DataAccessObject.Enums;
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
                if (page <= 0)
                {
                    page = 1;
                }
                var events = await _eventRepo.GetEvent();
                if (!string.IsNullOrEmpty(search))
                {
                    events = events.Where(e => (e.Title.Contains(search, StringComparison.OrdinalIgnoreCase)));
                }

                events = sort.ToLower().Trim() switch
                {
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
                if (eventEntity == null)
                {
                    res.Success = false;
                    res.Message = "Event not found.";
                    return res;
                }


                var eventDetails = new ViewEventDTO()
                {
                    Id = eventEntity.Id,
                    Title = eventEntity.Title,
                    Description = eventEntity.Description,
                    OrganizerId = eventEntity.OrganizerId,
                    OrganizerName = eventEntity.Organizer.Name, // Access the organizer's name
                    VenueId = eventEntity.VenueId,
                    VenueName = eventEntity.Venue.Name, // Access the venue's name
                    StartDate = eventEntity.StartDate, // Convert DateOnly to DateTime
                    EndDate = eventEntity.EndDate, // Convert DateOnly to DateTime
                    Image = eventEntity.ImageUrl,
                    Status = eventEntity.Status
                };

                res.Data = eventDetails;

                res.Success = true;
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

            if (eventDTO.StartDate.Date < DateTime.UtcNow.Date)
            {
                result.Success = false;
                result.Message = "StartDate cannot be in the past.";
                return result;
            }

            if (eventDTO.EndDate.Date < eventDTO.StartDate.Date)
            {
                result.Success = false;
                result.Message = "EndDate cannot be before StartDate.";
                return result;
            }

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
                        StartDate = eventDTO.StartDate,
                        EndDate = eventDTO.EndDate,
                        OrganizerId = eventDTO.OrganizerId,
                        VenueId = eventDTO.VenueId,
                        Description = eventDTO.Description,
                        Status = EventStatus.PENDING
                    };

                    // Save the event to the database
                    await _eventRepo.AddAsync(Event);


                    // Map to DTO
                    var newEvent = new ViewEventDTO()
                    {
                        Id = Event.Id,
                        Title = Event.Title,
                        Image = Event.ImageUrl,
                        StartDate = Event.StartDate,
                        EndDate = Event.EndDate,
                        OrganizerId = Event.OrganizerId,
                        VenueId = Event.VenueId,
                        Description = Event.Description,
                        Status = Event.Status
                    };
                    result.Data = newEvent;

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

        public async Task<ServiceResponse<bool>> AssignStaffToEventAsync(int staffId, int eventId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                // Get the event by ID
                var eventEntity = await _eventRepo.GetByIdAsync(eventId);

                if (eventEntity == null)
                {
                    response.Success = false;
                    response.Message = "Event not found.";
                    return response;
                }

                // Assign the staff to the event
                eventEntity.StaffId = staffId;

                // Save changes to the database
                await _eventRepo.UpdateAsync(eventEntity);

                response.Data = true;
                response.Success = true;
                response.Message = "Staff assigned to the event successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error assigning staff to the event.";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<EventStaffDTO?>> GetEventByStaffAsync(int staffId)
        {
            var response = new ServiceResponse<EventStaffDTO?>();

            try
            {
                // Retrieve the staff by their ID
                var staff = await _userRepo.GetByIdAsync(staffId);

                if (staff == null || staff.Role != Roles.STAFF)
                {
                    throw new UnauthorizedAccessException("User does not have the required Staff role.");
                }

                // Retrieve all events assigned to this staff member
                var events = await _eventRepo.GetEventsByStaffIdAsync(staffId);

                if (events.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No events found for this staff member.";
                    return response;
                }

                // Map the events to a list of EventDTO
                var eventDtos = events.Select(eventEntity => new EventDTO
                {
                    Id = eventEntity.Id,
                    Title = eventEntity.Title,
                    ImageUrl = eventEntity.ImageUrl,
                    StartDate = eventEntity.StartDate,
                    EndDate = eventEntity.EndDate,
                    VenueName = eventEntity.Venue.Name,
                    Status = eventEntity.Status,
                    Description = eventEntity.Description
                }).ToList();

                // Create the StaffEventsDTO
                var staffEventsDto = new EventStaffDTO
                {
                    Id = staff.Id,
                    Name = staff.Name,
                    Email = staff.Email,
                    AssignedEvents = eventDtos
                };

                response.Data = staffEventsDto;
                response.Success = true;
                response.Message = "Events retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving events for the staff member.";
                response.ErrorMessages = [ex.Message];
            }

            return response;
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


        public async Task<ServiceResponse<string>> DeleteEvent(int id)
        {
            var res = new ServiceResponse<string>();
            try
            {
                var eventExist = await _eventRepo.GetEventById(id);
                if (eventExist == null)
                {
                    res.Success = false;
                    res.Message = "Event not found!";
                }
                else
                {
                    await _eventRepo.DeleteEvent(id);
                }

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
                eventToUpdate.StartDate = eventDTO.StartDate;
                eventToUpdate.EndDate = eventDTO.EndDate;

                // If a new image is provided, upload it
                if (eventDTO.ImageFile != null)
                {
                    var imageUrl = await UploadImageCollection(eventDTO.ImageFile);
                    eventToUpdate.ImageUrl = imageUrl;
                }

                await _eventRepo.UpdateAsync(eventToUpdate);
                res.Data = new ViewEventDTO()
                {
                    Id = eventToUpdate.Id,
                    Title = eventToUpdate.Title,
                    Description = eventToUpdate.Description,
                    OrganizerId = eventToUpdate.OrganizerId,
                    OrganizerName = eventToUpdate.Organizer.Name, // Access the organizer's name
                    VenueId = eventToUpdate.VenueId,
                    VenueName = eventToUpdate.Venue.Name, // Access the venue's name
                    StartDate = eventToUpdate.StartDate, // Convert DateOnly to DateTime
                    EndDate = eventToUpdate.EndDate, // Convert DateOnly to DateTime
                    Image = eventToUpdate.ImageUrl,
                    Status = eventToUpdate.Status
                };

                res.Success = true;
                res.Message = "Event updated successfully";
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = e.InnerException != null
                    ? e.InnerException.Message + "\n" + e.StackTrace
                    : e.Message + "\n" + e.StackTrace;
            }

            return res;
        }

        public async Task<ServiceResponse<bool>> ChangeEventStatus(int eventId, ChangeEventStatusDTO statusDTO)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var eventToUpdate = await _eventRepo.GetEventById(eventId);
                if (eventToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "Event not found.";
                    return response;
                }

                // Validate the status if needed (e.g., check if it's a valid status)
                if (string.IsNullOrWhiteSpace(statusDTO.Status))
                {
                    response.Success = false;
                    response.Message = "Invalid status value.";
                    return response;
                }

                eventToUpdate.Status = statusDTO.Status;
                await _eventRepo.UpdateAsync(eventToUpdate);

                response.Success = true;
                response.Message = "Event status updated successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                // Log the exception details if needed
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
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
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

        public async Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetEventsByStatus(string status, int page,
            int pageSize)
        {
            var result = new ServiceResponse<PaginationModel<ViewEventDTO>>();

            try
            {
                var events = await _eventRepo.GetEventsByStatus(status);
                var map = _mapper.Map<IEnumerable<ViewEventDTO>>(events);
                var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                result.Data = paging;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.ErrorMessages = [ex.Message];
            }

            return result;
        }
    }
}