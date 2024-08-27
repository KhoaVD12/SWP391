using BusinessObject.Models.GiftReceptionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.GiftDTO
{
    public class ViewGiftDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int BoothId { get; set; }
        public string BoothName { get; set; }
        public int SponsorId { get; set; }
        public string SponsorName { get; set; }
        public int Quantity { get; set; }
    }
}
