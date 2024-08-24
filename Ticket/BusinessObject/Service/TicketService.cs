using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.TicketDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using DataAccessObject.Repo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Service
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepo _ticketRepo;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly IEventRepo _eventRepo;

        public TicketService(ITicketRepo repo, IMapper mapper, AppConfiguration configuration, IEventRepo eventRepo)
        {
            _appConfiguration = configuration;
            _ticketRepo = repo;
            _mapper = mapper;
            _eventRepo = eventRepo;
        }

        public async Task<ServiceResponse<ViewTicketDTO>> CreateTicket(CreateTicketDTO ticketDTO)
        {
            var res = new ServiceResponse<ViewTicketDTO>();
            try
            {
                var createResult = _mapper.Map<Ticket>(ticketDTO);
                var existTicket = await _ticketRepo.GetTicketByEventId(createResult.EventId);
                if (!await _ticketRepo.CheckEventPendingOrActive(createResult.EventId))
                {
                    res.Success = false;
                    res.Message = "Event ID Not Found or not In Pending/Active";
                    return res;
                }
                var eventTicket = await _eventRepo.GetEventById(createResult.EventId);
                if (existTicket.Any())
                {
                    res.Success = false;
                    res.Message = "Ticket with this event ID has already existed";
                    return res;
                }
                var minimumValidSaleEndDate = eventTicket.StartDate.AddMinutes(-10);
                var maximumValidSaleEndDate = eventTicket.StartDate.AddMinutes(-5);

                if (ticketDTO.TicketSaleEndDate < minimumValidSaleEndDate || ticketDTO.TicketSaleEndDate > maximumValidSaleEndDate)
                {
                    res.Success = false;
                    res.Message = "Ticket Sale EndDate must be 5-10 minutes before the Event StartDate.";
                    return res;
                }
                await _ticketRepo.CreateTicket(createResult);

                var result = _mapper.Map<ViewTicketDTO>(ticketDTO);
                result.Id = createResult.Id;
                res.Success = true;
                res.Message = "Ticket created successfully";
                res.Data = result;
            }
            catch (DbException e)
            {
                res.Success = false;
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

        public async Task<ServiceResponse<bool>> DeleteTicket(int id)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                var result = await _ticketRepo.DeleteTicket(id);
                res.Success = true;
                res.Message = "Ticket Deleted successfully";
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to delete Ticket:{e.Message}";
            }

            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewTicketDTO>>> GetAllTickets(int page, int pageSize)
        {
            var res = new ServiceResponse<PaginationModel<ViewTicketDTO>>();
            try
            {
                var tickets = await _ticketRepo.GetTicket();
                if (tickets.Any())
                {
                    var map = _mapper.Map<IEnumerable<ViewTicketDTO>>(tickets);
                    var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                    res.Data = paging;
                    res.Success = true;
                }
                else
                {
                    res.Success= false;
                    res.Message = "No Ticket";
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to get Ticket: {ex.Message}";
            }

            return res;
        }

        public async Task<ServiceResponse<IEnumerable<ViewTicketDTO>>> GetTicketByEventId(int eventId)
        {
            var res=new ServiceResponse<IEnumerable<ViewTicketDTO>>();
            try
            {
                var result = await _ticketRepo.GetTicketByEventId(eventId);
                if (result.Any())
                {
                    var map = _mapper.Map<IEnumerable<ViewTicketDTO>>(result);
                    res.Success= true;
                    res.Data= map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "No ticket with this Event Id";
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<ViewTicketDTO>> GetTicketById(int id)
        {
            var res = new ServiceResponse<ViewTicketDTO>();
            try
            {
                var result = await _ticketRepo.GetTicketById(id);
                if (result != null)
                {
                    var map = _mapper.Map<ViewTicketDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Ticket not Found";
                }
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }

            return res;
        }

        public async Task<ServiceResponse<ViewTicketDTO>> UpdateTicket(int id, CreateTicketDTO ticketDTO)
        {
            var res = new ServiceResponse<ViewTicketDTO>();
            try
            {
                var updateResult=_mapper.Map<Ticket>(ticketDTO);
                var existTicket = await _ticketRepo.GetTicketByEventId(updateResult.EventId);
                if (!await _ticketRepo.CheckEventPendingOrActive(updateResult.EventId))
                {
                    res.Success = false;
                    res.Message = "Event ID Not Found or not In Pending/Active";
                    return res;
                }
                var eventTicket = await _eventRepo.GetEventById(updateResult.EventId);
                var minimumValidSaleEndDate = eventTicket.StartDate.AddMinutes(-10);
                var maximumValidSaleEndDate = eventTicket.StartDate.AddMinutes(-5);
                if (ticketDTO.TicketSaleEndDate < minimumValidSaleEndDate || ticketDTO.TicketSaleEndDate > maximumValidSaleEndDate)
                {
                    res.Success = false;
                    res.Message = "Ticket Sale EndDate must be 5-10 minutes before the Event StartDate.";
                    return res;
                }
                updateResult.Id= id;
                await _ticketRepo.UpdateTicket(id, updateResult);
                var result = _mapper.Map<ViewTicketDTO>(updateResult);
                res.Success = true;
                res.Message = "Ticket updated successfully";
                res.Data = result;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to update Ticket:{ex.Message}";
            }

            return res;
        }
    }
}