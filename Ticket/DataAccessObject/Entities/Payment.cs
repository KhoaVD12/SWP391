﻿using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
