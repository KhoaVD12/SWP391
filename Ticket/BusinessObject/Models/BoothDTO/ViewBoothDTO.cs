using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.BoothDTO
{
    public class ViewBoothDTO
    {
        public int Id { get; set; }
        public int SponsorId { get; set; }

        public int EventId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Status { get; set; } = null!;

        public string Location { get; set; } = null!;
    }
}
