using BusinessObject.Models.BoothDTO;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IBoothService
    {
        public Task<ServiceResponse<PaginationModel<ViewBoothDTO>>> GetAllBooths(int page, int pageSize, string search, string sort);
        public Task<ServiceResponse<ViewBoothDTO>> GetBoothById(int id);
        public Task<ServiceResponse<ViewBoothDTO>> CreateBooth(CreateBoothDTO boothDTO);
        public Task<ServiceResponse<bool>> DeleteBooth(int id);
        public Task<ServiceResponse<ViewBoothDTO>> UpdateBooth(int id, CreateBoothDTO boothDTO);
        public Task<ServiceResponse<bool>> ChangeStatusBooth(int id, BoothStatusDTO boothDTO);

    }
}
