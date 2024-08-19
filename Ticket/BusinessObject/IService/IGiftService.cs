using BusinessObject.Models.GiftDTO;
using BusinessObject.Models.VenueDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IGiftService
    {
        Task<ServiceResponse<ViewGiftDTO>> CreateGift(CreateGiftDTO giftDTO);
        Task<ServiceResponse<PaginationModel<ViewGiftDTO>>> GetAllGifts(int page, int pageSize, string search, string sort);
        Task<ServiceResponse<bool>> DeleteGift(int id);
        Task<ServiceResponse<ViewGiftDTO>> UpdateGift(int id, CreateGiftDTO newVenue);
        Task<ServiceResponse<ViewGiftDTO>> GetGiftById(int id);
        Task<ServiceResponse<IEnumerable<ViewGiftDTO>>> GetGiftByBoothId(int boothId);
    }
}
