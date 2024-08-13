using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models.BoothDTO;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
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

        public async Task<CreateBoothDTO> CreateBooth(CreateBoothDTO boothDTO)
        {
            try
            {
                var createResult = _mapper.Map<Booth>(boothDTO);
                createResult.Status = "Pending";
                await _boothRepo.CreateBooth(createResult);
                var mapp = _mapper.Map<CreateBoothDTO>(createResult);
                return mapp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
