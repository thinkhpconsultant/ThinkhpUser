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
    public class ParaglidingTicketPurchaseController : Controller
    {
        #region Private Variables
        private readonly IParaglidingTicketPurchaseService _paraglidingTicketPurchaseService;
        private readonly ILogger<ParaglidingTicketPurchaseController> _logger;
        #endregion

        #region Constructors
        public ParaglidingTicketPurchaseController(IParaglidingTicketPurchaseService authService, ILogger<ParaglidingTicketPurchaseController> logger)
        {
            _paraglidingTicketPurchaseService = authService;
            _logger = logger;
            _logger.LogInformation(1, "NLog injected into TicketPurchaseController");
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("/paraglidingTicketPurchase")]
        public async Task<ActionResult> ParaglidingTicketPurchaseService(ParaglidingTicketPurchaseRequestModel requestModel)
        {
            var resMsg = new CommonApiResponseModel { };
            try
            {
                var responseModel = await _paraglidingTicketPurchaseService.ParaglidingTicketPurchase(requestModel);
                resMsg = responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("TicketPurchaseController ->  Login: Exception occur: ", ex.Message);
            }
            finally
            {
                _logger.LogInformation("TicketPurchaseController ->  Login: Finally executed: ");
            }
            return Ok(resMsg);
        }
    }
}
