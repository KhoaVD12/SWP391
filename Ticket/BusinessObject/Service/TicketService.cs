﻿using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.TicketDTO;
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
    public class TicketService:ITicketService
    {
        private readonly ITicketRepo _ticketRepo;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        public TicketService(ITicketRepo repo, IMapper mapper, AppConfiguration configuration)
        {
            _appConfiguration = configuration;
            _ticketRepo = repo;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CreateTicketDTO>> CreateTicket(CreateTicketDTO ticketDTO)
        {
            var res = new ServiceResponse<CreateTicketDTO>();
            try
            {
                var createResult=_mapper.Map<Ticket>(ticketDTO);
                await _ticketRepo.CreateTicket(createResult);
                var result = _mapper.Map<CreateTicketDTO>(ticketDTO);
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
            var res=new ServiceResponse<bool>();
            try
            {
                var result=await _ticketRepo.DeleteTicket(id);
                res.Success = true;
                res.Message = "Ticket Deleted successfully";
            }
            catch(Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to delete Ticket:{e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewTicketDTO>>> GetAllTickets(int page, int pageSize, string sort)
        {
            var res=new ServiceResponse<PaginationModel<ViewTicketDTO>>();
            try
            {
                var tickets = await _ticketRepo.GetTicket();
                tickets = sort.ToLower().Trim() switch
                {
                    "saleenddate" => tickets.OrderBy(e => e.TicketSaleEndDate),
                    "quantity" => tickets.OrderBy(t => t.Quantity),
                    "price" => tickets.OrderBy(t => t.Price),
                    _=>tickets.OrderBy(t=>t.Id).ToList(),
                };
                var map=_mapper.Map<IEnumerable<ViewTicketDTO>>(tickets);
                var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                res.Data = paging;
                res.Success = true;
            }
            catch(Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to get Ticket: {ex.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<ViewTicketDTO>> GetTicketById(int id)
        {
            var res=new ServiceResponse<ViewTicketDTO>();
            try
            {
                var result=await _ticketRepo.GetTicketById(id);
                if (result != null)
                {
                    var map=_mapper.Map<ViewTicketDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Ticket not Found";
                }
            }
            catch(Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<ViewTicketDTO>> UpdateTicket(int id, ViewTicketDTO ticketDTO)
        {
            var res=new ServiceResponse<ViewTicketDTO>();
            try
            {
                var updateResult=_mapper.Map<Ticket>(ticketDTO);
                await _ticketRepo.UpdateTicket(id, updateResult);
                var result=_mapper.Map<ViewTicketDTO>(updateResult);
                res.Success = true;
                res.Message = "Ticket updated successfully";
                res.Data = result;
            }
            catch(Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to update Ticket:{ex.Message}";
            }
            return res;
        }
    }
}
