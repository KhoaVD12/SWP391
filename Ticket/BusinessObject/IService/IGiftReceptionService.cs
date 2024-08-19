using BusinessObject.Models.GiftDTO;
using BusinessObject.Models.GiftReceptionDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IGiftReceptionService
    {
        Task<ServiceResponse<PaginationModel<ViewGiftReceptionDTO>>> GetReceptions(int page, int pageSize, string sort);
        Task<ServiceResponse<ViewGiftReceptionDTO>> CreateReception(CreateGiftReceptionDTO receptionDTO);
        Task<ServiceResponse<ViewGiftReceptionDTO>> GetReceptionById(int id);
        Task<ServiceResponse<IEnumerable<ViewGiftReceptionDTO>>> GetReceptionByAttendeeId(int attendeeId);
        Task<ServiceResponse<IEnumerable<ViewGiftReceptionDTO>>> GetReceptionByGiftId(int giftId);
        Task<ServiceResponse<bool>> DeleteGiftReception(int id);
    }
}
