﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;

namespace ThinkhpUserAPI.Repository.Interface
{
    public interface IUserRegistrationService
    {
        Task<CommonApiResponseModel> UserRegistration(UserRegistrationRequestModel model);
    }
}