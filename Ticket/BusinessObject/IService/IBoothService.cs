using BusinessObject.Models.BoothDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IBoothService
    {
        Task<CreateBoothDTO> CreateBooth(CreateBoothDTO boothDTO);
    }
}
