using BusinessObject.Models.GiftReceptionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.GiftDTO
{
    public class CreateGiftDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int BoothId { get; set; }
        public int Quantity { get; set; }
        public List<CreateGiftReceptionDTO> ReceptionDTO {  get; set; }
    }
    
}
