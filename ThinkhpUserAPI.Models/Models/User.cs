using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class User
{
    public long UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Address { get; set; }

    public string? MobileNumber { get; set; }

    public string? AlternateMobileNumber { get; set; }

    public string? Email { get; set; }

    public int? CityId { get; set; }

    public int? StateId { get; set; }

    public int? PinCode { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? InsertedOn { get; set; }

    public long? InsertedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public long? UpdatedBy { get; set; }

    public virtual CityMaster? City { get; set; }

    public virtual StateMaster? State { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
