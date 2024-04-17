using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class CityMaster
{
    public int CityId { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
