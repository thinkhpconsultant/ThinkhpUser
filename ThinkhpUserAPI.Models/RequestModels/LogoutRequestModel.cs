﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkhpUserAPI.Models.RequestModels
{
    public class LogoutRequestModel
    {
        public long UserId { get; set; }
        public string Token { get; set; }
    }
}
