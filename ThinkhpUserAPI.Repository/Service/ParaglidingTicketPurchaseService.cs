using Azure;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        //Main method to insert data from request model to database
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
                        long userId = Convert.ToInt64(responseOfInsertCustomer.Data);
                        var insertInPurchaseTicketTable = await InsertDataIntoParaglidingTicketPurchaseTable(requestModel, userId);
                        await InsertDataIntoParaglidingTicketPurchaseDetailTable(Convert.ToInt64(insertInPurchaseTicketTable.Data), userId, requestModel.AmountPerTicket ?? 0);
                        var successOfListEntry = InsertListOfCustomers(requestModel, Convert.ToInt64(insertInPurchaseTicketTable.Data), requestModel.AmountPerTicket ?? 0);

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

        #region Method For Validation
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
        #endregion

        #region Common Methods
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

        //To insert the list of customer into user table
        private async Task<CommonApiResponseModel> InsertListOfCustomers(ParaglidingTicketPurchaseRequestModel requestModel, long purchaseTicketId, decimal amountPerTicket)
        {
            if (requestModel.customerList != null)
            {
                foreach (var eachCustomer in requestModel.customerList)
                {
                    bool isCustomerNew = CheckIfCustomerIsNew(eachCustomer.MobileNumber, eachCustomer.FirstName, eachCustomer.LastName).Result;
                    bool isMobileNumberNew = CheckIfMobileNumberIsNew(eachCustomer.MobileNumber).Result;
                    if (isCustomerNew)
                    {
                        if (!isMobileNumberNew)
                            return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_duplicate_number_validation };
                        else
                        {
                            var responseFromCustomerRegister = CustomerRegistrationFromTicketPurchase(eachCustomer);
                            if (responseFromCustomerRegister.Result.StatusCode != 0)
                            {
                                return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
                            }
                            else
                            {
                                await InsertDataIntoParaglidingTicketPurchaseTable(requestModel, Convert.ToInt64(responseFromCustomerRegister.Result.Data));
                                await InsertDataIntoParaglidingTicketPurchaseDetailTable(purchaseTicketId, Convert.ToInt64(responseFromCustomerRegister.Result.Data), amountPerTicket);
                            }
                        }
                    }
                    else
                    {
                        var oldCustomer = _context.Users.Where(x => x.FirstName == eachCustomer.FirstName && x.LastName == eachCustomer.LastName && x.MobileNumber == eachCustomer.MobileNumber).FirstOrDefault();
                        if (oldCustomer != null)
                        {
                            await InsertDataIntoParaglidingTicketPurchaseTable(requestModel, oldCustomer.UserId);
                            await InsertDataIntoParaglidingTicketPurchaseDetailTable(purchaseTicketId, oldCustomer.UserId, amountPerTicket);

                        }
                    }
                }
                return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
            }
            else
                return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
        }

        private async Task<CommonApiResponseModel> InsertDataIntoParaglidingTicketPurchaseTable(ParaglidingTicketPurchaseRequestModel requestModel, long? userId)
        {
            ParaglidingTicketPurchase paraglidingTicketPurchase = new ParaglidingTicketPurchase()
            {
                UserId = userId,
                NoOfTicket = requestModel.NoOfTicket,
                AmountPerTicket = requestModel.AmountPerTicket,
                DiscountPercentage = requestModel.DiscountPercentage,
                DiscountAmount = requestModel.DiscountAmount,
                Cgstpercentage = requestModel.Cgstpercentage,
                Cgstamount = requestModel.Cgstamount,
                Sgstpercentage = requestModel.Sgstpercentage,
                Sgstamount = requestModel.Sgstamount,
                NetAmount = requestModel.NetAmount,
            };
            await _context.ParaglidingTicketPurchases.AddAsync(paraglidingTicketPurchase);
            await _context.SaveChangesAsync();

            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message, Data = paraglidingTicketPurchase.PurchasedTicketId };
        }

        private async Task<CommonApiResponseModel> InsertDataIntoParaglidingTicketPurchaseDetailTable(long PurchasedTicketId, long UserId, decimal AmountPerTicket)
        {
            ParaglidingTicketPurchaseDetail paraglidingTicketPurchaseDetail = new ParaglidingTicketPurchaseDetail()
            {
                PurchasedTicketId = PurchasedTicketId,
                UserId = UserId,
                NetAmount = AmountPerTicket
            };
            await _context.ParaglidingTicketPurchaseDetails.AddAsync(paraglidingTicketPurchaseDetail);
            await _context.SaveChangesAsync();
            await InsertDataIntoParaglidingTicketStatusTable(paraglidingTicketPurchaseDetail.PurchasedTicketDetailId);
            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
        }
        private async Task<CommonApiResponseModel> InsertDataIntoParaglidingTicketStatusTable(long PurchasedTicketDetailId)
        {
            ParaglidingTicketStatus paraglidingTicketStatuses = new ParaglidingTicketStatus()
            {
                PurchasedTicketDetailId = PurchasedTicketDetailId,
                IsTicketPurchased = true,
            };
            await _context.ParaglidingTicketStatuses.AddAsync(paraglidingTicketStatuses);
            await _context.SaveChangesAsync();

            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
        }
        #endregion
    }
}