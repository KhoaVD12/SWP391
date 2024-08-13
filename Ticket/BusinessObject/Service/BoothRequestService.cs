using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.BoothRequestDTO;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
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

        public async Task<CreateBoothRequestDTO> CreateBoothRequest(CreateBoothRequestDTO boothRequestDTO)
        {
            try
            {
                var createResult=_mapper.Map<BoothRequest>(boothRequestDTO);
                createResult.Status = "Pending";
                var checkExist = await _boothRequestRepo.CheckExistByBoothId(createResult.BoothId);
                if (checkExist)
                {
                    throw new Exception("Request with this Booth has already existed!");
                }
                await _boothRequestRepo.CreateBoothRequest(createResult);
                var res = _mapper.Map<CreateBoothRequestDTO>(createResult);
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteBoothRequest(int id)
        {
            try
            {
                return await _boothRequestRepo.DeleteBoothRequest(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ViewBoothRequestDTO>> GetAllBoothRequest()
        {
            try
            {
                var result=await _boothRequestRepo.GetAllBoothRequest();
                var mapp= _mapper.Map<IEnumerable<ViewBoothRequestDTO>>(result);
                return mapp;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
