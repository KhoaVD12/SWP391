﻿using BusinessObject.Models.BoothRequestDTO;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IBoothRequestService
    {
        public Task<ServiceResponse<PaginationModel<ViewBoothRequestDTO>>> GetAllBoothRequests(int page, int pageSize, string search, string sort);
        public Task<ServiceResponse<ViewBoothRequestDTO>> GetBoothRequestById(int id);
        public Task<ServiceResponse<CreateBoothRequestDTO>> CreateBoothRequest(CreateBoothRequestDTO boothRequestDTO);
        public Task<ServiceResponse<bool>> DeleteBoothRequest(int id);
        public Task<ServiceResponse<ViewBoothRequestDTO>> UpdateBoothRequest(int id, ViewBoothRequestDTO boothRequestDTO);
    }
}
