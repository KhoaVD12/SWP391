using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetAllEvents(int page, int pageSize, string search, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewEventDTO>>();
            try
            {
                var events = await _eventRepo.GetEvent();
                if (!string.IsNullOrEmpty(search))
                {
                    events = events.Where(e => e != null && (e.Title.Contains(search, StringComparison.OrdinalIgnoreCase)));
                        ;
                }
                events = sort.ToLower().Trim() switch
                {
                    "title" => events.OrderBy(e => e?.Title),
                    "startdate" => events.OrderBy(e => e?.StartDate),
                    "enddate"=>events.OrderBy(e=>e?.EndDate),
                    _ => events.OrderBy(e => e.Id).ToList()
                };

                var map = _mapper.Map<IEnumerable<ViewEventDTO>>(events);
                var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                res.Data = paging;
                res.Success = true;
             }
            catch (Exception e) 
            { 
                res.Success=false;
                res.Message = $"Fail to get Event: {e.Message}";
            }
            return res;
        }
        
        public async Task<ServiceResponse<ViewEventDTO>> GetEventById(int id)
        {
             var res = new ServiceResponse<ViewEventDTO>();
            try
            {
                var result = await _eventRepo.GetEventById(id);
                if(result != null)
                {
                    var mapp = _mapper.Map<ViewEventDTO>(result);
                    res.Success = true;
                    res.Data= mapp;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Event not Found";
                }
            }
            catch (Exception ex)
            {
                res.Success= false;
                res.Message=ex.Message;
            }
            return res;
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

        
        public async Task<ServiceResponse<bool>> DeleteEvent(int id)
        {
            var res= new ServiceResponse<bool>();
            try
            {
                await _eventRepo.DeleteEvent(id);
                res.Success = true;
                res.Message = "Event Deleted successfully";

            }
            catch (Exception ex)
            {
                res.Success=false; 
                res.Message=$"Fail to delete Event:{ex.Message}";
            }
            return res;
        }
        public async Task<ServiceResponse<ViewEventDTO>> UpdateEvent(int id, ViewEventDTO eventDTO)
        {
            var res = new ServiceResponse<ViewEventDTO>();
            try
            {
                    var updateResult = _mapper.Map<Event>(eventDTO);
                    updateResult.StartDate = DateOnly.FromDateTime(eventDTO.StartDate);
                    updateResult.EndDate = DateOnly.FromDateTime(eventDTO.EndDate);
                    await _eventRepo.UpdateEvent(id, updateResult);
                    var result = _mapper.Map<ViewEventDTO>(updateResult);
                    res.Success=true;
                    res.Message = "Event updated successfully";
                    res.Data=result;
                
            }
            catch (Exception ex)
            {
                res.Success=false;
                res.Message = $"Fail to update Event:{ex.Message}";
            }
            return res;
        }
    }
}