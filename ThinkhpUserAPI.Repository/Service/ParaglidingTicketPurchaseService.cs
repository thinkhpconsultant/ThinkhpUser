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

        public async Task<CommonApiResponseModel> ParaglidingTicketPurchaseInsert(ParaglidingTicketPurchaseRequestModel requestModel)
        {
            bool isCustomerNew = CheckIfCustomerIsNew(requestModel.MobileNumber, requestModel.FirstName, requestModel.LastName).Result;
            bool isMobileNumberNew = CheckIfMobileNumberIsNew(requestModel.MobileNumber).Result;
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
                    var responseOfInsertCustomer = await CustomerRegistrationFromTicketPurchase(newCustomer);
                    if (responseOfInsertCustomer.StatusCode == 0)
                    {
                        var successOfListEntry = InsertListOfCustomers(requestModel.customerList);
                        if (successOfListEntry.Status == 0)
                        {

                            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
                        }
                        else
                            return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
                    }
                    else
                        return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
                }
            }
            else
            {
                return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
            }
        }

        private async Task<bool> CheckIfCustomerIsNew(string MobileNumber, string FirstName, string LastName)
        {
            var temp = _context.Users.Where(x => x.FirstName.Equals(FirstName) && x.LastName.Equals(LastName) && x.MobileNumber.Equals(MobileNumber)).FirstOrDefault();
            if (temp == null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckIfMobileNumberIsNew(string mobileNumber)
        {
            var customers = _context.Users.Where(x => x.MobileNumber == mobileNumber).FirstOrDefault();
            if (customers == null)
                return true;
            else
                return false;
        }

        private async Task<CommonApiResponseModel> InsertListOfCustomers(List<CustomerRegistrationRequestModel> customerList)
        {
            foreach (var eachCustomer in customerList)
            {
                bool isCustomerNew = CheckIfCustomerIsNew(eachCustomer.MobileNumber, eachCustomer.FirstName, eachCustomer.LastName).Result;
                bool isMobileNumberDuplicate = CheckIfMobileNumberIsNew(eachCustomer.MobileNumber).Result;
                if (isCustomerNew)
                {
                    if (isMobileNumberDuplicate)
                        return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_duplicate_number_validation };
                }
                else
                {
                    var response = CustomerRegistrationFromTicketPurchase(eachCustomer);
                    if (response.Status != 0)
                    {
                        return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
                    }
                }
            }
            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
        }
    }
}