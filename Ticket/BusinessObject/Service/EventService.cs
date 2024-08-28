using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using DataAccessObject.Enums;
using BusinessObject.Models.TicketDTO;

namespace BusinessObject.Service
{
    public class EventService : IEventService
    {
        private readonly IUserRepo _userRepo;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly ITicketRepo _ticketRepo;
        private readonly ITicketService _ticketService;

        public EventService(IEventRepo repo, IMapper mapper,
            IUserRepo userRepo, ITicketRepo ticketRepo, ITicketService ticketService)
        {
            _eventRepo = repo;
            _mapper = mapper;
            _userRepo = userRepo;
            _ticketRepo = ticketRepo;
            _ticketService = ticketService;
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
                if (events == null)
                {
                    res.Success = false;
                    res.Message = "No events found.";
                    return res;
                }

                if (!string.IsNullOrEmpty(search))
                {
                    events = events.Where(e => e.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                events = sort.ToLower().Trim() switch
                {
                    "startdate" => events.OrderBy(e => e.StartDate),
                    "enddate" => events.OrderBy(e => e.EndDate),
                    _ => events.OrderBy(e => e.Id).ToList()
                };
                var eventDTOs=_mapper.Map<IEnumerable<ViewEventDTO >>(events);

                var paginationModel =
                    await Pagination.GetPaginationEnum(eventDTOs, page, pageSize);

                res.Data = paginationModel;
                res.Success = true;
                res.Message = "Events retrieved successfully!";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetEventsForGuests(int page, int pageSize,
            string search, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewEventDTO>>();

            try
            {
                if (page <= 0)
                {
                    page = 1;
                }

                // Retrieve all events
                var events = await _eventRepo.GetEventsForGuestsAsync();
                if (events == null)
                {
                    res.Success = false;
                    res.Message = "No events found.";
                    return res;
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    events = events.Where(e => e.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                // Apply sorting
                events = sort.ToLower().Trim() switch
                {
                    "startdate" => events.OrderBy(e => e.StartDate),
                    "enddate" => events.OrderBy(e => e.EndDate),
                    _ => events.OrderBy(e => e.Id)
                };

                var eventDTOs=_mapper.Map<IEnumerable<ViewEventDTO >>(events);

                // Apply pagination
                var paginationModel = await Pagination.GetPaginationEnum(eventDTOs, page, pageSize);

                res.Data = paginationModel;
                res.Success = true;
                res.Message = "Events retrieved successfully!";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.InnerException?.Message ?? ex.Message;
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
                    OrganizerName = eventEntity.Organizer.Name,
                    VenueId = eventEntity.VenueId,
                    VenueName = eventEntity.Venue.Name,
                    StartDate = eventEntity.StartDate,
                    EndDate = eventEntity.EndDate,
                    ImageURL = eventEntity.ImageUrl,
                    Presenter = eventEntity.Presenter,
                    Host = eventEntity.Host,
                    Status = eventEntity.Status,
                    Ticket = new ViewTicketDTO
                    {
                        Id = eventEntity.Ticket.Id,
                        EventId = eventEntity.Ticket.EventId,
                        Price = eventEntity.Ticket.Price,
                        Quantity = eventEntity.Ticket.Quantity,
                        TicketSaleEndDate = eventEntity.Ticket.TicketSaleEndDate
                    },
                    BoothNames = eventEntity.Booths.Select(b => b.Name).ToList()
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

            // Define the time zone
            var seAsiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert StartDate and EndDate to SE Asia Standard Time
            var startDateTime = TimeZoneInfo.ConvertTimeFromUtc(eventDTO.StartDate.ToUniversalTime(), seAsiaTimeZone);
            var endDateTime = TimeZoneInfo.ConvertTimeFromUtc(eventDTO.EndDate.ToUniversalTime(), seAsiaTimeZone);

            if (startDateTime.Date < DateTime.UtcNow.Date)
            {
                result.Success = false;
                result.Message = "StartDate cannot be in the past.";
                return result;
            }

            if (endDateTime.Date < startDateTime.Date)
            {
                result.Success = false;
                result.Message = "EndDate cannot be before StartDate.";
                return result;
            }

            var eventStatus = EventStatus.PENDING;

            if (eventDTO.StaffId.HasValue)
            {
                var isStaffAssigned = await _eventRepo.IsStaffAssignedToAnotherEventAsync(eventDTO.StaffId.Value);
                if (isStaffAssigned)
                {
                    result.Success = false;
                    result.Message = "The staff member is already assigned to another event with a conflicting status.";
                    return result;
                }

                // If StaffId is provided and the staff is not assigned, set status to "Ready"
                eventStatus = EventStatus.READY;
            }

            try
            {
                if (!string.IsNullOrEmpty(eventDTO.Title))
                {
                    var eventExist = await _eventRepo.CheckExistByDateAndVenue(null, eventDTO.StartDate,
                        eventDTO.EndDate, eventDTO.VenueId);
                    if (eventExist)
                    {
                        result.Success = false;
                        result.Message = "You have the Event with the same start date and venue";
                        return result;
                    }

                    var organizerExist = await _userRepo.GetByIdAsync(eventDTO.OrganizerId);
                    if (organizerExist == null)
                    {
                        result.Success = false;
                        result.Message = "Organizer not found!";
                        return result;
                    }

                    string imageUrl = null;
                    if (!string.IsNullOrEmpty(eventDTO.ImageUrl) && await IsValidImageUrlAsync(eventDTO.ImageUrl))
                    {
                        imageUrl = eventDTO.ImageUrl;
                    }
                    else if (!string.IsNullOrEmpty(eventDTO.ImageUrl))
                    {
                        result.Success = false;
                        result.Message = "Invalid image URL.";
                        return result;
                    }

                    var Event = new Event
                    {
                        Title = eventDTO.Title,
                        ImageUrl = imageUrl,
                        StartDate = startDateTime,
                        EndDate = endDateTime,
                        OrganizerId = eventDTO.OrganizerId,
                        StaffId = eventDTO.StaffId,
                        VenueId = eventDTO.VenueId,
                        Description = eventDTO.Description,
                        Status = eventStatus, // Set the status based on whether StaffId was provided
                        Presenter = eventDTO.Presenter,
                        Host = eventDTO.Host
                    };

                    await _eventRepo.AddAsync(Event);

                    var newTicket = new CreateTicketDTO
                    {
                        EventId = Event.Id,
                        Price = eventDTO.Price,
                        Quantity = eventDTO.Quantity,
                        TicketSaleEndDate = startDateTime.AddMinutes(-5)
                    };

                    var ticketResult = await _ticketService.CreateTicket(newTicket);
                    if (!ticketResult.Success)
                    {
                        result.Success = false;
                        result.Message = ticketResult.Message;
                        return result;
                    }

                    var newEvent = new ViewEventDTO
                    {
                        Id = Event.Id,
                        Title = Event.Title,
                        ImageURL = Event.ImageUrl,
                        StartDate = Event.StartDate,
                        EndDate = Event.EndDate,
                        OrganizerId = Event.OrganizerId,
                        OrganizerName = Event.Organizer.Name,
                        StaffId = Event.StaffId,
                        StaffName = Event.Staff?.Name,
                        VenueId = Event.VenueId,
                        VenueName = Event.Venue.Name,
                        Description = Event.Description,
                        Status = Event.Status,
                        Presenter = Event.Presenter,
                        Host = Event.Host,
                        Ticket = new ViewTicketDTO
                        {
                            EventId = Event.Id,
                            Price = newTicket.Price,
                            Quantity = newTicket.Quantity,
                            TicketSaleEndDate = newTicket.TicketSaleEndDate
                        }
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

        private async Task<bool> IsValidImageUrlAsync(string imageUrl)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult)
                || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)) return false;
            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            if (validExtensions.Contains(Path.GetExtension(uriResult.AbsolutePath).ToLower()))
            {
                try
                {
                    using var httpClient = new HttpClient();
                    // Set a timeout for the request
                    httpClient.Timeout = TimeSpan.FromSeconds(10); // Adjust timeout as needed

                    // Use HttpMethod.Get to get the content type reliably
                    var response = await httpClient.GetAsync(imageUrl);

                    // Ensure the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Check for specific image content types
                        var contentType = response.Content.Headers.ContentType?.MediaType;
                        if (contentType == "image/jpeg" || contentType == "image/png" ||
                            contentType == "image/gif" || contentType == "image/bmp" ||
                            contentType == "image/webp")
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    Console.WriteLine($"Error validating image URL: {ex.Message}");
                    return false;
                }
            }

            return false;
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

                // Convert UTC to SE Asia Standard Time
                var seAsiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, seAsiaTimeZone);

                // Prevent updates if the event has started
                if (eventToUpdate.StartDate <= currentLocalDateTime)
                {
                    res.Success = false;
                    res.Message = "You cannot update the event once the StartDate has come.";
                    return res;
                }

                // Check for staff assignment conflicts
                if (eventDTO.StaffId.HasValue && eventDTO.StaffId.Value != eventToUpdate.StaffId)
                {
                    // Check if the new staff member is assigned to another event with conflicting status
                    bool isStaffAssigned = await _eventRepo.IsStaffAssignedToAnotherEventAsync(eventDTO.StaffId.Value);
                    if (isStaffAssigned)
                    {
                        res.Success = false;
                        res.Message =
                            "The staff member is already assigned to another event with a conflicting status.";
                        return res;
                    }
                }

                // Prevent updates if another event exists with the same start date and venue
                var eventExist = await _eventRepo.CheckExistByDateAndVenue(id, (DateTime)eventDTO.StartDate,
                    (DateTime)eventDTO.EndDate, (int)eventDTO.VenueId);
                if (eventExist)
                {
                    res.Success = false;
                    res.Message = "You have another event with the same start date and venue.";
                    return res;
                }

                // Update the event properties
                eventToUpdate.Title = eventDTO.Title ?? eventToUpdate.Title;
                eventToUpdate.Description = eventDTO.Description ?? eventToUpdate.Description;
                eventToUpdate.VenueId = eventDTO.VenueId ?? eventToUpdate.VenueId;
                eventToUpdate.StartDate = eventDTO.StartDate ?? eventToUpdate.StartDate;
                eventToUpdate.EndDate = eventDTO.EndDate ?? eventToUpdate.EndDate;
                eventToUpdate.StaffId = eventDTO.StaffId ?? eventToUpdate.StaffId;
                eventToUpdate.ImageUrl = eventDTO.ImageUrl ?? eventToUpdate.ImageUrl;
                eventToUpdate.Presenter = eventDTO.Presenter ?? eventToUpdate.Presenter;
                eventToUpdate.Host = eventDTO.Host ?? eventToUpdate.Host;

                // Validation: Ensure EndDate is not before StartDate
                if (eventToUpdate.EndDate < eventToUpdate.StartDate)
                {
                    res.Success = false;
                    res.Message = "EndDate cannot be before StartDate.";
                    return res;
                }

                // Validate the new image URL if it was provided
                if (!string.IsNullOrEmpty(eventDTO.ImageUrl) && !await IsValidImageUrlAsync(eventDTO.ImageUrl))
                {
                    res.Success = false;
                    res.Message = "Invalid image URL.";
                    return res;
                }

                // Change status to "Ready" if staff is assigned and was not assigned before
                if (eventDTO.StaffId.HasValue && eventToUpdate.StaffId == null)
                {
                    eventToUpdate.Status = EventStatus.READY;
                }

                // Update the event in the repository
                await _eventRepo.UpdateAsync(eventToUpdate);

                var updatedEvent = new ViewEventDTO
                {
                    Id = eventToUpdate.Id,
                    Title = eventToUpdate.Title,
                    Description = eventToUpdate.Description,
                    OrganizerId = eventToUpdate.OrganizerId,
                    OrganizerName = eventToUpdate.Organizer.Name,
                    VenueId = eventToUpdate.VenueId,
                    VenueName = eventToUpdate.Venue.Name,
                    StartDate = eventToUpdate.StartDate,
                    EndDate = eventToUpdate.EndDate,
                    ImageURL = eventToUpdate.ImageUrl,
                    Status = eventToUpdate.Status, // Ensure status is updated correctly
                    StaffId = eventToUpdate.StaffId,
                    StaffName = eventToUpdate.Staff?.Name,
                    Presenter = eventToUpdate.Presenter,
                    Host = eventToUpdate.Host,
                    Ticket = new ViewTicketDTO
                    {
                        EventId = eventToUpdate.Id,
                        Price = eventToUpdate.Ticket.Price,
                        Quantity = eventToUpdate.Ticket.Quantity,
                        TicketSaleEndDate = eventToUpdate.Ticket.TicketSaleEndDate
                    }
                };

                res.Data = updatedEvent;
                res.Success = true;
                res.Message = "Event updated successfully!";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.InnerException?.Message ?? ex.Message;
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

        public async Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetEventsByStatus(string status, int page,
            int pageSize)
        {
            var result = new ServiceResponse<PaginationModel<ViewEventDTO>>();

            try
            {
                var events = await _eventRepo.GetEventsByStatusAsync(status);
                var eventDTOs=_mapper.Map<IEnumerable<ViewEventDTO >>(events);
                var paging = await Pagination.GetPaginationEnum(eventDTOs, page, pageSize);
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

        public async Task<ServiceResponse<PaginationModel<ViewOrganizerEventDTO>>> GetEventByOrganizer(int organizerId,
            int page, int pageSize)
        {
            var res = new ServiceResponse<PaginationModel<ViewOrganizerEventDTO>>();
            try
            {
                var result = await _eventRepo.GetEventByOrganizer(organizerId);
                if (result.Any())
                {
                    var map = _mapper.Map<IEnumerable<ViewOrganizerEventDTO>>(result);
                    var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                    res.Data = paging;
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Event not Found";
                    return res;
                }
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = e.Message;
            }

            return res;
        }
    }
}