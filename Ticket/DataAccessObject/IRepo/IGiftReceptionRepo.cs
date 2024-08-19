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
        Task<IEnumerable<GiftReception>> GetReceptionByGiftId(int giftId);
        Task<IEnumerable<GiftReception>> GetReceptionByAttendeeId(int attendeeId);
        Task<bool> CheckAttendeeExist(int attendeeId);
        Task<bool> CheckGiftExist(int giftId);
        Task<bool> CheckMaxGiftQuantity(int giftId);
        Task<bool> DeleteGiftReception(int id);
    }
}
