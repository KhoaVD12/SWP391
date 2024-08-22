using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.BoothRequestDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using DataAccessObject.Repo;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Service
{
    public class BoothRequestService:IBoothRequestService
    {
        private readonly IMapper _mapper;
        private readonly IBoothRequestRepo _boothRequestRepo;
        private readonly AppConfiguration _appConfiguration;
        public BoothRequestService(IBoothRequestRepo repo, IMapper mapper, AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            _mapper = mapper;
            _boothRequestRepo = repo;
        }

        public async Task<ServiceResponse<bool>> ChangeBoothRequestStatus(int id, BoothRequestStatusDTO boothRequestStatus)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                var exist = await _boothRequestRepo.GetBoothRequestById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                
                exist.Status=boothRequestStatus.Status;
                await _boothRequestRepo.UpdateBoothRequest(id, exist);
                
                res.Success = true;
                res.Message = "Request Status updated successfully";
                res.Data = true;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to update Request Status:{ex.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<ViewBoothRequestDTO>> CreateBoothRequest(CreateBoothRequestDTO boothRequestDTO)
        { 
            var res=new ServiceResponse<ViewBoothRequestDTO>();
            try
            {
                var createResult=_mapper.Map<BoothRequest>(boothRequestDTO);
                createResult.Status = "Pending";
                var checkExist = await _boothRequestRepo.CheckExistByBoothId(createResult.BoothId);
                if (checkExist)
                {
                    res.Success = false;
                    res.Message = "Request with this booth existed";
                    return res;
                }
                await _boothRequestRepo.CreateBoothRequest(createResult);
                var result = _mapper.Map<ViewBoothRequestDTO>(createResult);
                res.Success = true;
                res.Message = "Request created successfully";
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

        public async Task<ServiceResponse<bool>> DeleteBoothRequest(int id)
        {
            var res=new ServiceResponse<bool>();
            try
            {
                var exist = await _boothRequestRepo.GetBoothRequestById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                await _boothRequestRepo.DeleteBoothRequest(id);
                res.Success = true;
                res.Message = "Request Deleted successfully";
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to delete Request:{ex.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewBoothRequestDTO>>> GetAllBoothRequests(int page, int pageSize, string search, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewBoothRequestDTO>>();
            try
            {
                var requests=await _boothRequestRepo.GetAllBoothRequest();
                if (!string.IsNullOrEmpty(search))
                {
                    requests = requests.Where(r => r != null && (r.Status.Contains(search, StringComparison.OrdinalIgnoreCase)));
                }
                requests = sort.ToLower().Trim() switch
                {
                    "requestdate" => requests.OrderBy(r => r.RequestDate),
                    _=>requests.OrderBy(r => r.Id),
                };
                if (requests != null && requests.Any())
                {
                    var mapp = _mapper.Map<IEnumerable<ViewBoothRequestDTO>>(requests);
                    var paging = await Pagination.GetPaginationEnum(mapp, page, pageSize);
                    res.Data = paging;
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Message = "No request found";
                    return res;
                }
            }
            catch(Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to get Request: {e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<IEnumerable<ViewBoothRequestDTO>>> GetBoothRequestByEventId(int eventId)
        {
            var res = new ServiceResponse<IEnumerable<ViewBoothRequestDTO>>();
            try
            {
                var result = await _boothRequestRepo.GetBoothRequestByEventId(eventId);
                if(result != null && result.Any())
                {
                    var map = _mapper.Map<IEnumerable<ViewBoothRequestDTO>>(result);
                    res.Success= true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Request not Found";
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

        public async Task<ServiceResponse<ViewBoothRequestDTO>> GetBoothRequestById(int id)
        {
            var res = new ServiceResponse<ViewBoothRequestDTO>();
            try
            {
                var result = await _boothRequestRepo.GetBoothRequestById(id);
                if(result!= null)
                {
                    var map = _mapper.Map<ViewBoothRequestDTO>(result);
                    res.Success = true;
                    res.Data = map;
                    return res;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Request not Found";
                }
            }
            catch(Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<IEnumerable<ViewBoothRequestDTO>>> GetBoothRequestBySponsorId(int sponsorId)
        {
            var res = new ServiceResponse<IEnumerable<ViewBoothRequestDTO>>();
            try
            {
                var result = await _boothRequestRepo.GetBoothRequestBySponsorId(sponsorId);
                if (result != null && result.Any())
                {
                    var map = _mapper.Map<IEnumerable<ViewBoothRequestDTO>>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Request not Found";
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

        public async Task<ServiceResponse<ViewBoothRequestDTO>> UpdateBoothRequest(int id, CreateBoothRequestDTO boothRequestDTO)
        {
            var res= new ServiceResponse<ViewBoothRequestDTO>();
            try
            {
                var exist = await _boothRequestRepo.GetBoothRequestById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                var updateResult = _mapper.Map<BoothRequest>(boothRequestDTO);
                updateResult.RequestDate = DateTime.Now;
                updateResult.Id= id;
                await _boothRequestRepo.UpdateBoothRequest(id, updateResult);
                var result = _mapper.Map<ViewBoothRequestDTO>(updateResult);
                res.Success = true;
                res.Message = "Request updated successfully";
                res.Data = result;
            }
            catch(Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to update Request:{ex.Message}";
            }
            return res;
        }
    }
}
