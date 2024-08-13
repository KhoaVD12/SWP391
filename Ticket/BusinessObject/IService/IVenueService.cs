using BusinessObject.Models.VenueDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IVenueService
    {
        Task<CreateVenueDTO> CreateVenue(CreateVenueDTO venueDTO);
    }
}
