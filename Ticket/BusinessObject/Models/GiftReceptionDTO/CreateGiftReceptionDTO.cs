using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.GiftReceptionDTO
{
    public class CreateGiftReceptionDTO
    {
            public int AttendeeId { get; set; }

            public int GiftId { get; set; }

            public DateTime ReceptionDate { get; set; }
    }
}
