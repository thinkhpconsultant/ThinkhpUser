using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class StateMaster
{
    public int StateId { get; set; }

    public string? Name { get; set; }

    public string? StateCode { get; set; }

    public string? Posno { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
