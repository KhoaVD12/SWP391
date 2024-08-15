using AutoMapper;
using Azure;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.VenueDTO;
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
    public class VenueService : IVenueService
    {
        private readonly IMapper _mapper;
        private readonly IVenueRepo _venueRepo;
        private readonly AppConfiguration _appConfiguration;
        public VenueService(IMapper mapper, IVenueRepo venueRepo, AppConfiguration configuration)
        {
            _mapper = mapper;
            _venueRepo = venueRepo;
            _appConfiguration = configuration;
        }
        public async Task<ServiceResponse<ViewVenueDTO>> CreateVenue(CreateVenueDTO venueDTO)
        {
            var res = new ServiceResponse<ViewVenueDTO>();
            try
            {
                var mapp = _mapper.Map<Venue>(venueDTO);

                await _venueRepo.CreateVenue(mapp);

                var result = _mapper.Map<ViewVenueDTO>(mapp);

                res.Success = true;
                res.Data = result;
                res.Message = "Venue created successfully";
                
            }
            catch (DbException e)
            {
                res.Success = false;
                res.Message = "Database error occurred.";
                res.ErrorMessages = new List<string> { e.Message };
            }
            catch(Exception e)
            {
                res.Success = false;
                res.Message = "An error occurred.";
                res.ErrorMessages = new List<string> { e.Message };
            }
            return res;
        }

        public async Task<ServiceResponse<bool>> DeleteVenue(int id)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                await _venueRepo.DeleteVenue(id);
                res.Success = true;
                res.Message = "Venue Deleted successfully";
            }
            catch(Exception e)
            {
                res.Success=false;
                res.Message = $"Fail to delete Venue:{e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewVenueDTO>>> GetAllVenues(int page, int pageSize, string search, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewVenueDTO>>();
            try
            {
                var venues = await _venueRepo.GetAllVenues();
                if (!string.IsNullOrEmpty(search))
                {
                    venues = venues.Where(x => x != null && (x.Name.Contains(search,StringComparison.OrdinalIgnoreCase)));

                }
                venues = sort.ToLower() switch
                {
                    "name"=>venues.OrderBy(v=>v?.Name),
                    _=>venues.OrderBy(v=>v.Id).ToList()
                };
                var result = _mapper.Map<IEnumerable<ViewVenueDTO>>(venues);
                var paging = await Pagination.GetPaginationEnum(result, page, pageSize);
                res.Data = paging;
                res.Success=true;
            }
            catch(Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to retrieve Venue:{e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<ViewVenueDTO>> GetVenueById(int id)
        {
            var res=new ServiceResponse<ViewVenueDTO>();
            try
            {
                var result = await _venueRepo.GetVenueById(id);
                if (result != null)
                {
                    var map = _mapper.Map<ViewVenueDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Venue not Found";
                }
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = e.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<ViewVenueDTO>> UpdateVenue(int id, CreateVenueDTO newVenue)
        {
            var res = new ServiceResponse<ViewVenueDTO>();
            try
            {
                    var mapp = _mapper.Map<Venue>(newVenue);
                    mapp.Id = id;
                    await _venueRepo.UpdateVenue(id, mapp);
                    var result = _mapper.Map<ViewVenueDTO>(mapp);
                    res.Success = true;
                    res.Message = "Venue updated successfully";
                    res.Data = result;
            }
            catch (Exception e)
            {
                res.Success=false;
                res.Message = $"Fail to update Venue:{e.Message}";
            }
            return res;
        }
    }
}
