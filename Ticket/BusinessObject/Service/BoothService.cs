﻿using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.BoothDTO;
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
    public class BoothService:IBoothService
    {
        private readonly IBoothRepo _boothRepo;
        private readonly AppConfiguration _appConfiguration;
        private readonly IMapper _mapper;
        public BoothService(IBoothRepo repo, AppConfiguration appConfiguration, IMapper mapper)
        {
            _boothRepo = repo;
            _appConfiguration = appConfiguration;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CreateBoothDTO>> CreateBooth(CreateBoothDTO boothDTO)
        {
            var res = new ServiceResponse<CreateBoothDTO>();
            try
            {
                var createResult = _mapper.Map<Booth>(boothDTO);
                createResult.Status = "Pending";
                var nameExist = await _boothRepo.CheckExistByName(createResult.Name);
                if (nameExist)
                {
                    res.Success = false;
                    res.Message = "Name existed";
                    return res;
                }
                await _boothRepo.CreateBooth(createResult);
                var mapp = _mapper.Map<CreateBoothDTO>(createResult);
                res.Success = true;
                res.Message = "Booth created successfully";
                res.Data = mapp;
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

        public async Task<ServiceResponse<bool>> DeleteBooth(int id)
        {
            var res=new ServiceResponse<bool>();
            try
            {
                await _boothRepo.DeleteBooth(id);
                res.Success = true;
                res.Message = "Booth Deleted successfully";
            }
            catch(Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to delete Booth:{ex.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<PaginationModel<ViewBoothDTO>>> GetAllBooths(int page, int pageSize, string search, string sort)
        {
            var res=new ServiceResponse<PaginationModel<ViewBoothDTO>>();
            try
            {
                var booths = await _boothRepo.GetBooth();
                if (!string.IsNullOrEmpty(search))
                {
                    booths = booths.Where(b => b != null && (b.Name.Contains(search, StringComparison.OrdinalIgnoreCase)||
                                                            b.Location.Contains(search, StringComparison.OrdinalIgnoreCase)));
                }
                booths = sort.ToLower().Trim() switch
                {
                    "name" => booths.OrderBy(b => b.Name),
                    "location"=>booths.OrderBy(b=>b.Location),
                    _=>booths.OrderBy(b=>b.Id).ToList(),
                };
                var map = _mapper.Map<IEnumerable<ViewBoothDTO>>(booths);
                var paging = await Pagination.GetPaginationEnum(map, page, pageSize);
                res.Data = paging;
                res.Success = true;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Message = $"Fail to get Booth: {e.Message}";
            }
            return res;
        }

        public async Task<ServiceResponse<ViewBoothDTO>> GetBoothById(int id)
        {
            var res=new ServiceResponse<ViewBoothDTO>();
            try
            {
                var result=await _boothRepo.GetBoothById(id);
                if (result != null)
                {
                    var map=_mapper.Map<ViewBoothDTO>(result);
                    res.Success = true;
                    res.Data = map;
                }
                else
                {
                    res.Success = false;
                    res.Message = "Booth not Found";
                }
            }
            catch(Exception e)
            {
                res.Success = false;
                res.Message = e.Message;
            }
            return res;
        }

        public async Task<ServiceResponse<ViewBoothDTO>> UpdateBooth(int id, ViewBoothDTO boothDTO)
        {
            var res = new ServiceResponse<ViewBoothDTO>();
            try
            {
                var updateResult = _mapper.Map<Booth>(boothDTO);
                await _boothRepo.UpdateBooth(id, updateResult);
                var result = _mapper.Map<ViewBoothDTO>(updateResult);
                res.Success = true;
                res.Message = "Booth updated successfully";
                res.Data = result;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = $"Fail to update Booth:{ex.Message}";
            }
            return res;
        }
    }
}
