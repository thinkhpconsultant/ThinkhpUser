using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;

namespace ThinkhpUserAPI.Repository.Interface
{
    public interface IAuthService
    {
        Task<CommonApiResponseModel> Login(AuthenticateRequestModel model);
        Task<CommonApiResponseModel> Logout(LogoutRequestModel model);
    }
}