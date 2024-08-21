using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IBoothRepo:IGenericRepo<Booth>
    {
        Task CreateBooth(Booth booth);
        Task<IEnumerable<Booth>> GetBooth();
        Task<Booth> GetBoothById(int id);
        Task<bool> DeleteBooth(int id);
        Task UpdateBooth(int id, Booth booth);
        Task<bool> CheckExistByName(string inputString);
        Task<bool> CheckEventExist(int eventId);
        Task<bool> CheckSponsorExist(int sponsorId);
        Task<bool> CheckBoothExist(int eventId, int sponsorId);
    }
}
