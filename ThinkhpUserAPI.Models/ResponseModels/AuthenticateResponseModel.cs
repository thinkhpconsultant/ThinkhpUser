using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkhpUserAPI.Models.ResponseModels
{
    public class AuthenticateResponseModel
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        //public string Role { get; set; } = "Everyone";
        public bool IsActive { get; set; } = false;
        public string Token { get; set; } = "";
        public string TokenExpireTime { get; set; } = "";
    }
}
