using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int Role { get; set; }

    public bool? IsBanned { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
