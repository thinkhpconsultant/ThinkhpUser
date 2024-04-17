using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;
using ThinkhpUserAPI.Repository.Interface;
namespace ThinkhpUserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRegistrationController : Controller
    {
        #region Private Variables
        private readonly ICustomerRegistrationService _authService;
        private readonly ILogger<CustomerRegistrationController> _logger;
        #endregion

        #region Constructors
        public CustomerRegistrationController(ICustomerRegistrationService authService, ILogger<CustomerRegistrationController> logger)
        {
            _authService = authService;
            _logger = logger;
            _logger.LogInformation(1, "NLog injected into CustomerRegistrationController");
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("/customerRegistration")]
        public async Task<ActionResult> CustomerRegistration(CustomerRegistrationRequestModel user)
        {
            var resMsg = new CommonApiResponseModel { };
            try
            {
                var respoenseModel = await _authService.CustomerRegistration(user);
                resMsg = respoenseModel;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AccountController ->  Login: Exception occur: ", ex.Message);
            }
            finally
            {
                _logger.LogInformation("AccountController ->  Login: Finally executed: ");
            }
            return Ok(resMsg);
        }

    }
}
