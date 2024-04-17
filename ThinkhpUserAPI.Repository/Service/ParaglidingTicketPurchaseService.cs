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


        public async Task<CommonApiResponseModel> CustomerRegistrationFromTicketPurchase(CustomerRegistrationRequestModel customerRegistrationModel)
        {
            User newCustomer = new()
            {
                FirstName = customerRegistrationModel.FirstName,
                LastName = customerRegistrationModel.LastName,
                Address = customerRegistrationModel.Address,
                MobileNumber = customerRegistrationModel.MobileNumber,
                AlternateMobileNumber = customerRegistrationModel.AlternateMobileNumber,
                Email = customerRegistrationModel.Email,
                CityId = customerRegistrationModel.CityId,
                StateId = customerRegistrationModel.StateId,
                PinCode = customerRegistrationModel.PinCode,
                IsDeleted = false,
                InsertedBy = 1,
                InsertedOn = DateTime.UtcNow,
            };
            await _context.Users.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.SC_Msg_Ins, Data = newCustomer.UserId };
        }

        public async Task<CommonApiResponseModel> ParaglidingTicketPurchase(ParaglidingTicketPurchaseRequestModel requestModel)
        {

            bool isCustomerNew = CheckIfCustomerIsNew(requestModel).Result;
            bool isMobileNumberNew = CheckIfMobileNumberIsNew(requestModel).Result;
            if (isCustomerNew)
            {
                if (!isMobileNumberNew)
                    return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_duplicate_number_validation };
                else
                {
                    CustomerRegistrationRequestModel newCustomer = new()
                    {
                        FirstName = requestModel.FirstName,
                        LastName = requestModel.LastName,
                        Address = requestModel.Address,
                        MobileNumber = requestModel.MobileNumber,
                        AlternateMobileNumber = requestModel.AlternateMobileNumber,
                        Email = requestModel.Email,
                        CityId = requestModel.CityId,
                        StateId = requestModel.StateId,
                        PinCode = requestModel.PinCode,
                        IsDeleted = false,
                        InsertedBy = 1,
                        InsertedOn = DateTime.UtcNow,
                    };
                    await CustomerRegistrationFromTicketPurchase(newCustomer);
                }
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
                return true;
            else
                return false;
        }
    }
}