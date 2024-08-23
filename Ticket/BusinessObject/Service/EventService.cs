﻿using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccessObject.Enums;

namespace BusinessObject.Service
{
    public class EventService : IEventService
    {
        private readonly IUserRepo _userRepo;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public EventService(IEventRepo repo, IMapper mapper, Cloudinary cloudinary,
            IUserRepo userRepo)
        {
            _eventRepo = repo;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _userRepo = userRepo;
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
                    ImageURL = eventEntity.ImageUrl,
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

                    // Upload image if ImageUrl is provided
                    string imageUrl = null;

                    // Handle Image URL validation
                    if (!string.IsNullOrEmpty(eventDTO.ImageUrl))
                    {
                        if (await IsValidImageUrlAsync(eventDTO.ImageUrl))
                        {
                            imageUrl = await UploadImageFromUrl(eventDTO.ImageUrl);
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Invalid image URL.";
                            return result;
                        }
                    }

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
                        ImageURL = Event.ImageUrl,
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

        private async Task<bool> IsValidImageUrlAsync(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (validExtensions.Contains(Path.GetExtension(uriResult.AbsolutePath).ToLower()))
                {
                    try
                    {
                        using (var httpClient = new HttpClient())
                        {
                            var response = await httpClient.SendAsync(
                                new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Head, imageUrl));
                            var contentType = response.Content.Headers.ContentType.MediaType;
                            return contentType.StartsWith("image/");
                        }
                    }
                    catch
                    {
                        return false; // Handle exceptions as needed
                    }
                }
            }

            return false;
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

        public async Task<string> UploadImageFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("Image URL cannot be null or empty.", nameof(imageUrl));
            }

            try
            {
                using var httpClient = new HttpClient();
                var imageData = await httpClient.GetByteArrayAsync(imageUrl);

                using var stream = new MemoryStream(imageData);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription("image", stream),
                    Transformation = new Transformation().Crop("fill").Gravity("face")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.Url.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload image from URL.", ex);
            }
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
                // Retrieve the event to update
                var eventToUpdate = await _eventRepo.GetEventById(id);
                if (eventToUpdate == null)
                {
                    res.Success = false;
                    res.Message = "Event not found!";
                    return res;
                }

                // Update the fields that can be changed
                eventToUpdate.Title = eventDTO.Title ?? eventToUpdate.Title;
                eventToUpdate.Description = eventDTO.Description ?? eventToUpdate.Description;
                eventToUpdate.VenueId = eventDTO.VenueId != 0 ? eventDTO.VenueId : eventToUpdate.VenueId;
                eventToUpdate.StartDate =
                    eventDTO.StartDate != default ? eventDTO.StartDate : eventToUpdate.StartDate;
                eventToUpdate.EndDate = eventDTO.EndDate != default ? eventDTO.EndDate : eventToUpdate.EndDate;

                // If a new image URL is provided, upload it
                if (!string.IsNullOrEmpty(eventDTO.ImageUrl))
                {
                    // Validate the image URL and upload
                    var imageUrl = await UploadImageFromUrl(eventDTO.ImageUrl);
                    eventToUpdate.ImageUrl = imageUrl;
                }

                // Save the updated event to the database
                await _eventRepo.UpdateAsync(eventToUpdate);

                // Prepare the response DTO
                res.Data = new ViewEventDTO()
                {
                    Id = eventToUpdate.Id,
                    Title = eventToUpdate.Title,
                    Description = eventToUpdate.Description,
                    OrganizerId = eventToUpdate.OrganizerId,
                    OrganizerName = eventToUpdate.Organizer.Name,
                    VenueId = eventToUpdate.VenueId,
                    VenueName = eventToUpdate.Venue.Name,
                    StartDate = eventToUpdate.StartDate, // Convert DateOnly to DateTime if needed
                    EndDate = eventToUpdate.EndDate, // Convert DateOnly to DateTime if needed
                    ImageURL = eventToUpdate.ImageUrl,
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