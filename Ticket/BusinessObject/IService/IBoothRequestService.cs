using BusinessObject.Models.BoothRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IBoothRequestService
    {
        Task<CreateBoothRequestDTO> CreateBoothRequest(CreateBoothRequestDTO boothRequestDTO);
        Task<IEnumerable<ViewBoothRequestDTO>>GetAllBoothRequest();
        Task<bool> DeleteBoothRequest(int id);
    }
}
