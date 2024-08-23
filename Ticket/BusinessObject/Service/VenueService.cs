using AutoMapper;
using Azure;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.VenueDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using DataAccessObject.Repo;
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
                if (string.IsNullOrWhiteSpace(mapp.Name) || !mapp.Name.Any(char.IsLetter) || mapp.Name.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
                {
                    res.Success = false;
                    res.Message = "Invalid name. Name must contain at least one letter and no special characters.";
                    return res;
                }
                if(await _venueRepo.CheckNameExist(mapp.Name))
                {
                    res.Success = false;
                    res.Message = "Name existed";
                    return res;
                }
                mapp.Status=VenueStatus.Opened.ToString();
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
                var exist = await _venueRepo.GetVenueById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
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
                    venues = venues.Where(x => x != null && (x.Name.Contains(search,StringComparison.OrdinalIgnoreCase)||
                    x.Status.Contains(search, StringComparison.OrdinalIgnoreCase)));

                }
                venues = sort.ToLower() switch
                {
                    "name"=>venues.OrderBy(v=>v?.Name),
                    _=>venues.OrderBy(v=>v.Id).ToList()
                };
                if (venues.Any())
                {
                    var result = _mapper.Map<IEnumerable<ViewVenueDTO>>(venues);
                    var paging = await Pagination.GetPaginationEnum(result, page, pageSize);
                    res.Data = paging;
                    res.Success = true;
                }
                else
                {
                    res.Success=false;
                    res.Message = "No Venue found";
                    return res;
                }
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
                    return res;
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
                var exist = await _venueRepo.GetVenueById(id);
                if (string.IsNullOrWhiteSpace(newVenue.Name) || !newVenue.Name.Any(char.IsLetter) || newVenue.Name.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
                {
                    res.Success = false;
                    res.Message = "Invalid name. Name must contain at least one letter and no special characters.";
                    return res;
                }
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                var mapp = _mapper.Map<Venue>(newVenue);
                mapp.Id = id;
                if (await _venueRepo.CheckNameExist(mapp.Name))
                {
                    res.Success = false;
                    res.Message = "Name existed";
                    return res;
                }
                mapp.Status=exist.Status;
                mapp.Id=exist.Id;
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

        public async Task<ServiceResponse<bool>> ChangeVenueStatus(int id, VenueStatusDTO venueStatus)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                var exist = await _venueRepo.GetVenueById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                if (venueStatus.Status.Equals(VenueStatus.Opened.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    exist.Status = VenueStatus.Opened.ToString();
                }
                else if (venueStatus.Status.Equals(VenueStatus.Closed.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    exist.Status = VenueStatus.Closed.ToString();
                }
                else
                {
                    res.Success = false;
                    res.Message = "Invalid Status";
                    return res;
                }
                exist.Status=venueStatus.Status;
                await _venueRepo.UpdateVenue(id, exist);
                
                res.Success = true;
                res.Message = "Venue Status updated successfully";
                res.Data = true;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to update Venue Status:{e.Message}";
            }
            return res;
        }
    }
}
