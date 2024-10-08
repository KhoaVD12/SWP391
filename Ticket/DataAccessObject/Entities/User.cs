﻿namespace DataAccessObject.Entities;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
    public string Status { get; set; } = null!;

    public virtual ICollection<BoothRequest> BoothRequests { get; set; } = new List<BoothRequest>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Event> AssignedEvents { get; set; } = new List<Event>();


}
