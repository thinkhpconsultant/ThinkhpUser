using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;

namespace ThinkhpUserAPI.Repository.Interface
{
    public interface ICustomerRegistrationService
    {
        Task<CommonApiResponseModel> CustomerRegistration(CustomerRegistrationRequestModel model);
    }
}
