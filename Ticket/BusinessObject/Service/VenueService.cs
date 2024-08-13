using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.VenueDTO;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Service
{
    public class VenueService : IVenueService
    {
        private readonly IMapper _mapper;
        private readonly IVenueRepo _venueRepo;
        public VenueService(IMapper mapper, IVenueRepo venueRepo)
        {
            _mapper = mapper;
            _venueRepo = venueRepo;
        }
        public async Task<CreateVenueDTO> CreateVenue(CreateVenueDTO venueDTO)
        {
            try
            {
                var mapp = _mapper.Map<Venue>(venueDTO);
                await _venueRepo.CreateVenue(mapp);
                var res = _mapper.Map<CreateVenueDTO>(mapp);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
