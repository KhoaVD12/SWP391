using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IBoothRequestRepo:IGenericRepo<BoothRequest>
    {
        Task CreateBoothRequest(BoothRequest request);
        Task<IEnumerable<BoothRequest>> GetAllBoothRequest();
        Task<bool> CheckExistByBoothId(int boothId);
        Task<bool> DeleteBoothRequest(int id);
        Task UpdateBoothRequest(int id, BoothRequest boothRequest);
        Task<BoothRequest> GetBoothRequestById(int id);
    }
}
