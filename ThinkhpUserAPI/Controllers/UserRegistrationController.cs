using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThinkhpUserAPI.Models;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;
using ThinkhpUserAPI.Repository.Interface;
namespace ThinkhpUserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : Controller
    {
        #region Private Variables
        private readonly IUserRegistrationService _authService;
        private readonly ILogger<UserRegistrationController> _logger;
        #endregion

        #region Constructors
        public UserRegistrationController(IUserRegistrationService authService, ILogger<UserRegistrationController> logger)
        {
            _authService = authService;
            _logger = logger;
            _logger.LogInformation(1, "NLog injected into UserRegistrationController");
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("/userRegistration")]
        public async Task<ActionResult> UserRegistration(UserRegistrationRequestModel user)
        {
            var resMsg = new CommonApiResponseModel { };
            try
            {
                var responseModel = await _authService.UserRegistration(user);
                resMsg = responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("UserRegistrationController ->  Login: Exception occur: ", ex.Message);
            }
            finally
            {
                _logger.LogInformation("UserRegistrationController ->  Login: Finally executed: ");
            }
            return Ok(resMsg);
        }
    }
}
