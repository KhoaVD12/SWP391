using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.TicketDTO;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
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

        public async Task<CreateTicketDTO> CreateTicket(CreateTicketDTO ticketDTO)
        {
            try
            {
                
                var createResult=_mapper.Map<Ticket>(ticketDTO);
                await _ticketRepo.CreateTicket(createResult);
                var res = _mapper.Map<CreateTicketDTO>(ticketDTO);
                return res;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
