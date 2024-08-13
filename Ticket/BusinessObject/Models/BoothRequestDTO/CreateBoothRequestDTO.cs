using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.BoothRequestDTO
{
    public class CreateBoothRequestDTO
    {
        public int SponsorId { get; set; }

        public int BoothId { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
