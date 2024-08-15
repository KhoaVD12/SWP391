using BusinessObject.Models.UserDTO;
using BusinessObject.Models.VenueDTO;
using BusinessObject.Responses;
using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IVenueService
    {
        Task <ServiceResponse<CreateVenueDTO>>  CreateVenue(CreateVenueDTO venueDTO);
        Task<ServiceResponse<PaginationModel<ViewVenueDTO>>>  GetAllVenues(int page, int pageSize, string search, string sort);
        Task <ServiceResponse<bool>> DeleteVenue(int id);
        Task<ServiceResponse<ViewVenueDTO>> UpdateVenue(int id, ViewVenueDTO newVenue);
        Task<ServiceResponse<ViewVenueDTO>> GetVenueById(int id);
    }
}
