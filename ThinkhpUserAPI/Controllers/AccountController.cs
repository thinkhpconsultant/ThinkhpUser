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
    public class AccountController : Controller
    {
        #region Private Variables
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;
        #endregion

        #region Constructors
        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
            _logger.LogInformation(1, "NLog injected into AccountController");
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("/authenticate")]
        public async Task<ActionResult> authenticate([FromBody] AuthenticateRequestModel user)
        {
            var resMsg = new CommonApiResponseModel { };
            try
            {
                if (!ModelState.IsValid)
                {
                    string valMsg = (ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).FirstOrDefault()) ?? string.Empty;
                    resMsg.StatusCode = 1;
                    resMsg.Message = valMsg;
                }

                var loggedInUser = await _authService.Login(user);
                resMsg = loggedInUser;
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

        [AllowAnonymous]
        [HttpPost]
        [Route("/logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequestModel user)
        {
            var resMsg = new CommonApiResponseModel { };
            try
            {
                var loggedInUser = await _authService.Logout(user);
                resMsg = loggedInUser;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AccountController ->  Logout: Exception occur: ", ex.Message);
            }
            finally
            {
                _logger.LogInformation("AccountController ->  Logout: Finally executed: ");
            }
            return Ok(resMsg);
        }
    }
}
