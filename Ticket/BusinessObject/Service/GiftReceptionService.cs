using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.BoothDTO;
using BusinessObject.Models.GiftReceptionDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
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
    public class GiftReceptionService:IGiftReceptionService
    {
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly IGiftReceptionRepo _giftReceptionRepo;
        public GiftReceptionService(IMapper mapper, AppConfiguration configuration, IGiftReceptionRepo repo)
        {
            _appConfiguration = configuration;
            _mapper = mapper;
            _giftReceptionRepo = repo;
        }

        public async Task<ServiceResponse<ViewGiftReceptionDTO>> CreateReception(CreateGiftReceptionDTO receptionDTO)
        {
            var res=new ServiceResponse<ViewGiftReceptionDTO>();
            try
            {
                
                var createResult = _mapper.Map<GiftReception>(receptionDTO);
                if (!await _giftReceptionRepo.CheckAttendeeExist(createResult.AttendeeId)|| !await _giftReceptionRepo.CheckGiftExist(createResult.GiftId))
                {
                    res.Success = false;
                    res.Message = "Attendee or/and Gift not exist";
                    return res;
                }
                createResult.ReceptionDate = DateTime.Now;
                await _giftReceptionRepo.CreateGiftReception(createResult);
                var result = _mapper.Map<ViewGiftReceptionDTO>(createResult);
                res.Success = true;
                res.Message = "Reception created successfully";
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

        public async Task<ServiceResponse<ViewGiftReceptionDTO>> GetReceptionByAttendeeId(int attendeeId)
        {
            var res = new ServiceResponse<ViewGiftReceptionDTO>();
            try
            {
                var result = await _giftReceptionRepo.GetReceptionByAttendeeId(attendeeId);
                if (result != null)
                {
                    var map = _mapper.Map<ViewGiftReceptionDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Reception not Found";
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

        public async Task<ServiceResponse<ViewGiftReceptionDTO>> GetReceptionByGiftId(int giftId)
        {
            var res = new ServiceResponse<ViewGiftReceptionDTO>();
            try
            {
                var result = await _giftReceptionRepo.GetReceptionByGiftId(giftId);
                if (result != null)
                {
                    var map = _mapper.Map<ViewGiftReceptionDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Reception not Found";
                    return res ;
                }
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = e.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<ViewGiftReceptionDTO>> GetReceptionById(int id)
        {
            var res = new ServiceResponse<ViewGiftReceptionDTO>();
            try
            {
                var result = await _giftReceptionRepo.GetReceptionById(id);
                if (result != null)
                {
                    var map = _mapper.Map<ViewGiftReceptionDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Reception not Found";
                    return res ;
                }
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = e.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewGiftReceptionDTO>>> GetReceptions(int page, int pageSize, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewGiftReceptionDTO>>();
            try
            {
                var receptions = await _giftReceptionRepo.GetReceptions();
                receptions = sort.ToLower().Trim() switch
                {
                    "receptiondate" => receptions.OrderBy(g => g.ReceptionDate),
                    _ => receptions.OrderBy(g => g.Id).ToList(),
                };
                var map = _mapper.Map<IEnumerable<ViewGiftReceptionDTO>>(receptions);
                var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                res.Data = paging;
                res.Success = true;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to get Reception: {e.Message}";
            }
            return res;
        }
    }
}
