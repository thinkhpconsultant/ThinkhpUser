using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class UserLogInToken
{
    public long? UserTokenId { get; set; }

    public long? UserId { get; set; }

    public string? Token { get; set; }

    public DateTime? TokenExpireTime { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? InsertedOn { get; set; }

    public long? InsertedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public long? UpdatedBy { get; set; }
}
