using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.VenueDTO
{
    public class ViewVenueDTO
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Status { get; set; } = null!;
    }
}
