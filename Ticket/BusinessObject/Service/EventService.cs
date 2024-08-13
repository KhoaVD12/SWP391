using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
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
        public async Task<IEnumerable<ViewEventDTO>> GetAllEvents()
        {
            try
            {
                var result = await _eventRepo.GetEvent();
                var map = _mapper.Map<IEnumerable<ViewEventDTO>>(result);
                return map;
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        public async Task<CreateEventDTO> CreateEvent(CreateEventDTO eventDTO)
        {

            try
            {
                var createResult = _mapper.Map<Event>(eventDTO);
                createResult.StartDate = DateOnly.FromDateTime(eventDTO.StartDate);
                createResult.EndDate = DateOnly.FromDateTime(eventDTO.EndDate);
                createResult.Status = "Pending";
                var checkExistTitle = await _eventRepo.CheckExistByTitle(createResult.Title);
                if (checkExistTitle)
                {
                    throw new Exception("Title Existed!!");
                }
                if (createResult.StartDate >= createResult.EndDate)
                {
                    throw new Exception("Start Date can not be bigger than or equal End Date");
                }
                await _eventRepo.AddAsync(createResult);

                var res = _mapper.Map<CreateEventDTO>(createResult);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
