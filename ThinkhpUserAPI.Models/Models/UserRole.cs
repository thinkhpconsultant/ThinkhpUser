using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class UserRole
{
    public long UserRoleId { get; set; }

    public long? UserId { get; set; }

    public int? RoleId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? InsertedOn { get; set; }

    public long? InsertedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
