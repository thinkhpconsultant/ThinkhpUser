using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkhpUserAPI.Models.Helper;
using ThinkhpUserAPI.Models.Models;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;
using ThinkhpUserAPI.Repository.Interface;

namespace ThinkhpUserAPI.Repository.Service
{
    public class ParaglidingTicketPurchaseService : IParaglidingTicketPurchaseService
    {
        private readonly ThinkHPUsersContext _context;
        private readonly IConfiguration _configuration;

        public ParaglidingTicketPurchaseService(ThinkHPUsersContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<CommonApiResponseModel> ParaglidingTicketPurchase(ParaglidingTicketPurchaseRequestModel requestModel)
        {

            bool isCustomerNew = CheckIfCustomerIsNew(requestModel).Result;
            bool isMobileNumberDuplicate = CheckIfMobileNumberIsNew(requestModel).Result;
            if (isCustomerNew)
            {
                if (isMobileNumberDuplicate)
                    return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_duplicate_number_validation };
            }
            else
            {

            }
            return new CommonApiResponseModel();
        }

        private async Task<bool> CheckIfCustomerIsNew(ParaglidingTicketPurchaseRequestModel requestModel)
        {
            var temp = _context.Users.Where(x => x.FirstName.Equals(requestModel.FirstName) && x.LastName.Equals(requestModel.LastName) && x.MobileNumber.Equals(requestModel.MobileNumber)).FirstOrDefault();
            if (temp == null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckIfMobileNumberIsNew(ParaglidingTicketPurchaseRequestModel requestModel)
        {
            var customers = _context.Users.Where(x => x.MobileNumber == requestModel.MobileNumber).FirstOrDefault();
            if (customers == null)
                return false;
            else
                return true;
        }
    }
}