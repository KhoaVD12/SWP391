using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.TicketDTO
{
    public class CreateTicketDTO
    {
        public decimal Price { get; set; }

        public int EventId { get; set; }

        public int Quantity { get; set; }

        public DateTime TicketSaleEndDate { get; set; }
    }
}
