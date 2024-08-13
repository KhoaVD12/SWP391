using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using BusinessObject.Responses;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly Cloudinary _cloudinary;

        public EventService(IEventRepo repo, AppConfiguration configuration, IMapper mapper, Cloudinary cloudinary)
        {
            _eventRepo = repo;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _appConfiguration = configuration;
        }

        public async Task<IEnumerable<ViewEventDTO>> GetAllEvents()
        {
            try
            {
                var result = await _eventRepo.GetEvent();
                var map = _mapper.Map<IEnumerable<ViewEventDTO>>(result);
                return map;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ServiceResponse<CreateEventDTO>> CreateEvent(CreateEventDTO eventDTO)
        {
            var result = new ServiceResponse<CreateEventDTO>();
            try
            {
                if (eventDTO.Title != null)
                {
                    var eventExist = await _eventRepo.CheckExistByTitle(eventDTO.Title);
                    if (eventExist != null)
                    {
                        result.Success = false;
                        result.Message = "Event with the same name already exist!";
                    }
                    else
                    {
                        {
                            var imageURl = await UploadImageCollection(eventDTO.ImageUrl);

                            var Event = new Event
                            {
                                Title = eventDTO.Title,
                                ImageUrl = imageURl,
                                StartDate = DateOnly.FromDateTime(eventDTO.StartDate),
                                EndDate = DateOnly.FromDateTime(eventDTO.EndDate),
                                Status = "Pending"
                            };
                            await _eventRepo.AddAsync(Event);

                            var res = _mapper.Map<CreateEventDTO>(Event);

                            result.Data = res;
                        }

                        result.Success = true;
                        result.Message = "Create Event successfully!";
                    }
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

        public async Task<ViewEventDTO> GetEventById(int id)
        {
            try
            {
                var result = await _eventRepo.GetEventById(id);
                var mapp = _mapper.Map<ViewEventDTO>(result);
                return mapp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEvent(int id)
        {
            try
            {
                return await _eventRepo.DeleteEvent(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ViewEventDTO> UpdateEvent(int id, ViewEventDTO eventDTO)
        {
            try
            {
                var exist = await _eventRepo.GetEventById(id);
                if (exist != null)
                {
                    var updateResult = _mapper.Map<Event>(exist);
                    updateResult.StartDate = DateOnly.FromDateTime(eventDTO.StartDate);
                    updateResult.EndDate = DateOnly.FromDateTime(eventDTO.EndDate);
                    await _eventRepo.UpdateEvent(updateResult);
                    var res = _mapper.Map<ViewEventDTO>(updateResult);
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return null;
        }
    }
}