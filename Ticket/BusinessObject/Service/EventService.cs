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

namespace BusinessObject.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        public EventService(IEventRepo repo, AppConfiguration configuration, IMapper mapper)
        {
            _eventRepo = repo;
            _mapper = mapper;
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
        public async Task<ServiceResponse<CreateEventDTO>> CreateEvent(CreateEventDTO eventDTO)
        {
            var res = new ServiceResponse<CreateEventDTO>();
            try
            {
                var createResult = _mapper.Map<Event>(eventDTO);
                createResult.StartDate = DateOnly.FromDateTime(eventDTO.StartDate);
                createResult.EndDate = DateOnly.FromDateTime(eventDTO.EndDate);
                createResult.Status = "Pending";
                var checkExistTitle = await _eventRepo.CheckExistByTitle(createResult.Title);
                if (checkExistTitle)
                {
                    res.Success = false;
                    res.Message = "Title existed";
                    return res;
                }
                if (createResult.StartDate >= createResult.EndDate)
                {
                    res.Success = false;
                    res.Message = "Start Date can not be bigger than or equal End Date";
                    return res;
                }
                await _eventRepo.AddAsync(createResult);

                var result = _mapper.Map<CreateEventDTO>(createResult);
                res.Success=true;
                res.Message = "Event created successfully";
                res.Data = result;
                
            }
            catch (DbException e)
            {
                res.Success=false;
                res.Message = "Db error?";
                res.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = "An error occurred.";
                res.ErrorMessages = new List<string> { e.Message };
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
