using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IGiftReceptionRepo:IGenericRepo<GiftReception>
    {
        Task CreateGiftReception(GiftReception reception);
        Task<IEnumerable<GiftReception>> GetReceptions();
        Task<GiftReception> GetReceptionById(int id);
        Task<GiftReception> GetReceptionByGiftId(int giftId);
        Task<GiftReception> GetReceptionByAttendeeId(int attendeeId);
        Task<bool> CheckAttendeeExist(int attendeeId);
        Task<bool> CheckGiftExist(int giftId);
    }
}
