using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.GiftDTO;
using BusinessObject.Models.GiftReceptionDTO;
using BusinessObject.Models.VenueDTO;
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
    public class GiftService:IGiftService
    {
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly IGiftRepo _giftRepo;
        private readonly IGiftReceptionService _giftReceptionService;
        public GiftService(IGiftRepo repo, AppConfiguration configuration, IMapper mapper, IGiftReceptionService giftReceptionService)
        {
            _appConfiguration = configuration;
            _mapper = mapper;
            _giftRepo = repo;
            _giftReceptionService = giftReceptionService;
        }

        public async Task<ServiceResponse<ViewGiftDTO>> CreateGift(CreateGiftDTO giftDTO)
        {
            var res = new ServiceResponse<ViewGiftDTO>();
            try
            {
                var mapp = _mapper.Map<Gift>(giftDTO);
                var giftExist = await _giftRepo.CheckExistByNameAndBooth(mapp.Name, mapp.BoothId);
                if(giftExist)
                {
                    res.Success = false;
                    res.Message = "Gift existed";
                    return res;
                }
                await _giftRepo.CreateGift(mapp);
                
                var result = _mapper.Map<ViewGiftDTO>(mapp);

                res.Success = true;
                res.Data = result;
                res.Message = "Gift created successfully";

            }
            catch (DbException e)
            {
                res.Success = false;
                res.Message = "Database error occurred.";
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

        public async Task<ServiceResponse<bool>> DeleteGift(int id)
        {
            var res = new ServiceResponse<bool>();
            try
            {
                var exist = await _giftRepo.GetGiftById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                await _giftRepo.DeleteGift(id);
                res.Success = true;
                res.Message = "Gift Deleted successfully";
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to delete Gift:{e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewGiftDTO>>> GetAllGifts(int page, int pageSize, string search, string sort)
        {
            var res = new ServiceResponse<PaginationModel<ViewGiftDTO>>();
            try
            {
                var gifts = await _giftRepo.GetAllGifts();
                if (!string.IsNullOrEmpty(search))
                {
                    gifts = gifts.Where(x => x != null && (x.Name.Contains(search, StringComparison.OrdinalIgnoreCase)));

                }
                gifts = sort.ToLower() switch
                {
                    "quantity" => gifts.OrderBy(v => v?.Quantity),
                    "name" => gifts.OrderBy(v => v?.Name),
                    _ => gifts.OrderBy(v => v.Id).ToList()
                };
                if(gifts.Any()&&gifts!=null)
                {
                    var result = _mapper.Map<IEnumerable<ViewGiftDTO>>(gifts);
                    var paging = await Pagination.GetPaginationEnum(result, page, pageSize);
                    res.Data = paging;
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Message = "No Gift found";
                }
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to retrieve Gift:{e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<ViewGiftDTO>> GetGiftById(int id)
        {
            var res = new ServiceResponse<ViewGiftDTO>();
            try
            {
                var result = await _giftRepo.GetGiftById(id);
                if (result != null)
                {
                    var map = _mapper.Map<ViewGiftDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Gift not Found";
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
        public async Task<ServiceResponse<IEnumerable<ViewGiftDTO>>> GetGiftByBoothId(int boothId)
        {
            var res = new ServiceResponse<IEnumerable<ViewGiftDTO>>();
            try
            {
                var result = await _giftRepo.GetGiftByBoothId(boothId);
                if (result != null&&result.Any())
                {
                    var map = _mapper.Map<IEnumerable<ViewGiftDTO>>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Gift not Found";
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
        public async Task<ServiceResponse<IEnumerable<ViewGiftDTO>>> GetGiftsBySponsorId(int sponsorId)
        {
            var res = new ServiceResponse<IEnumerable<ViewGiftDTO>>();
            try
            {
                var gifts = await _giftRepo.GetGiftsBySponsorId(sponsorId);
                if (!gifts.Any())
                {
                    res.Success = false;
                    res.Message = "No gifts found for this sponsor.";
                    return res;
                }

                var giftDtos = _mapper.Map<IEnumerable<ViewGiftDTO>>(gifts);
                res.Data = giftDtos;
                res.Success = true;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"An error occurred: {e.Message}";
            }
            return res;
        }
        public async Task<ServiceResponse<ViewGiftDTO>> UpdateGift(int id, CreateGiftDTO newGift)
        {
            var res = new ServiceResponse<ViewGiftDTO>();
            try
            {
                var exist = await _giftRepo.GetGiftById(id);
                if (exist == null)
                {
                    res.Success = false;
                    res.Message = "Id not found";
                    return res;
                }
                var mapp = _mapper.Map<Gift>(newGift);
                mapp.Id = id;
                var giftExist = await _giftRepo.CheckExistByNameAndBooth(mapp.Name, mapp.BoothId);
                if (giftExist)
                {
                    res.Success = false;
                    res.Message = "Gift existed";
                    return res;
                }
                await _giftRepo.UpdateGift(id, mapp);
                var result = _mapper.Map<ViewGiftDTO>(mapp);
                res.Success = true;
                res.Message = "Gift updated successfully";
                res.Data = result;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to update Gift:{e.Message}";
            }
            return res;
        }
    }
}
