using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.TicketDTO
{
    public class CreateTicketDTO
    {
        [Range(10000, 10000000, ErrorMessage = "Price must be at least 10,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "EventId is required.")]
        public int EventId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ticket sale end date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime TicketSaleEndDate { get; set; }
    }
}