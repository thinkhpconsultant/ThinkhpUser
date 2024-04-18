using Microsoft.Extensions.Configuration;
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


        //Main method to insert data from request model to database
        public async Task<CommonApiResponseModel> ParaglidingTicketPurchaseInsert(ParaglidingTicketPurchaseRequestModel requestModel)
        {
            try
            {


                long userId = 0;
                var customerSearchOfRegistration = await InsertOrFindTheUserAndReturnIdOfUser(requestModel);
                if (customerSearchOfRegistration != null)
                {
                    if (customerSearchOfRegistration.StatusCode == 0)
                        userId = Convert.ToInt64(customerSearchOfRegistration.Data);
                }
                var insertInPurchaseTicketTable = await InsertDataIntoParaglidingTicketPurchaseTable(requestModel, userId);
                await InsertDataIntoParaglidingTicketPurchaseDetailTable(Convert.ToInt64(insertInPurchaseTicketTable.Data), userId, requestModel.AmountPerTicket ?? 0);
                var successOfListEntry = await InsertListOfCustomers(requestModel, Convert.ToInt64(insertInPurchaseTicketTable.Data));

                if (successOfListEntry.StatusCode == 0)
                {
                    return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
                }
                else
                    return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Method For Validation
        private bool CheckIfCustomerIsNew(string MobileNumber, string FirstName, string LastName)
        {
            var temp = _context.Users.Where(x => x.FirstName.Equals(FirstName) && x.LastName.Equals(LastName) && x.MobileNumber.Equals(MobileNumber)).FirstOrDefault();
            if (temp == null)
                return true;
            else
                return false;
        }
        private bool CheckIfMobileNumberIsNew(string mobileNumber)
        {
            var customers = _context.Users.Where(x => x.MobileNumber == mobileNumber).FirstOrDefault();
            if (customers == null)
                return true;
            else
                return false;
        }
        #endregion

        #region Common Methods
        // To insert or find the user from data & return the UserId
        private async Task<CommonApiResponseModel> InsertOrFindTheUserAndReturnIdOfUser(ParaglidingTicketPurchaseRequestModel requestModel)
        {
            try
            {

                long userId = 0;
                bool isCustomerNew = CheckIfCustomerIsNew(requestModel.MobileNumber, requestModel.FirstName, requestModel.LastName);
                bool isMobileNumberNew = CheckIfMobileNumberIsNew(requestModel.MobileNumber);
                CustomerRegistrationRequestModel newCreatedCustomer = new CustomerRegistrationRequestModel();

                #region Check if customer is New
                if (isCustomerNew)
                {
                    if (!isMobileNumberNew)
                        return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_duplicate_number_validation };
                    else
                        newCreatedCustomer = await CreateNewCustomerFromParaglidingRequestModel(requestModel);
                }
                #endregion

                if (newCreatedCustomer != null) // if this object is null that means user is not new
                {
                    var responseOfInsertCustomer = await CustomerRegistrationFromTicketPurchase(newCreatedCustomer);// To insert customer in USER table
                    if (responseOfInsertCustomer.StatusCode != 0) // Failed
                        return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Customer_Registration_Failed };
                    else // Success
                    {
                        userId = Convert.ToInt64(responseOfInsertCustomer.Data);
                        return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Customer_Registration_Success, Data = userId };
                    }
                }
                else
                {
                    long userIdFromData = await FindTheUserFromUsersData(requestModel.MobileNumber);
                    if (userIdFromData != 0)
                    {
                        userId = userIdFromData;
                        return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Customer_Search_Success, Data = userId };
                    }
                    else
                        return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Customer_Search_Fail };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //To convert the customer into CustomerRegistrationRequestModel so we can use common method to insert customers
        private async Task<CustomerRegistrationRequestModel> CreateNewCustomerFromParaglidingRequestModel(ParaglidingTicketPurchaseRequestModel requestModel)
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
            return newCustomer;
        }

        //Insert the customer into User table
        public async Task<CommonApiResponseModel> CustomerRegistrationFromTicketPurchase(CustomerRegistrationRequestModel customerRegistrationModel)
        {
            try
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
            catch (Exception)
            {

                throw;
            }

        }

        //To insert the list of customer into user table
        private async Task<CommonApiResponseModel> InsertListOfCustomers(ParaglidingTicketPurchaseRequestModel requestModel, long purchaseTicketId)
        {
            if (requestModel.customerList != null)
            {
                foreach (var eachCustomer in requestModel.customerList)
                {
                    try
                    {


                        bool isCustomerNew = CheckIfCustomerIsNew(eachCustomer.MobileNumber, eachCustomer.FirstName, eachCustomer.LastName);
                        bool isMobileNumberNew = CheckIfMobileNumberIsNew(eachCustomer.MobileNumber);
                        if (isCustomerNew)
                        {
                            if (!isMobileNumberNew)
                                return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_duplicate_number_validation };
                            else
                            {
                                var responseFromCustomerRegister = await CustomerRegistrationFromTicketPurchase(eachCustomer);
                                if (responseFromCustomerRegister.StatusCode != 0)
                                    return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.TXT_Ticket_Purchase_Error_Message };
                                else
                                    await InsertDataIntoParaglidingTicketPurchaseDetailTable(purchaseTicketId, Convert.ToInt64(responseFromCustomerRegister.Data), requestModel.AmountPerTicket ?? 0);
                            }
                        }
                        else
                        {
                            var oldCustomer = _context.Users.Where(x => x.FirstName == eachCustomer.FirstName && x.MobileNumber == eachCustomer.MobileNumber).FirstOrDefault();
                            if (oldCustomer != null)
                            {
                                await InsertDataIntoParaglidingTicketPurchaseDetailTable(purchaseTicketId, oldCustomer.UserId, requestModel.AmountPerTicket ?? 0);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
            }
            else
                return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.TXT_Ticket_Purchase_Success_Message };
        }


        //Insert data into ParaglidingTicketPurchase Table
        private async Task<CommonApiResponseModel> InsertDataIntoParaglidingTicketPurchaseTable(ParaglidingTicketPurchaseRequestModel requestModel, long? userId)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        //Insert data into ParaglidingTicketPurchaseDetail Table
        private async Task<CommonApiResponseModel> InsertDataIntoParaglidingTicketPurchaseDetailTable(long PurchasedTicketId, long UserId, decimal AmountPerTicket)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        //Insert data into ParaglidingTicketStatus Table
        private async Task<CommonApiResponseModel> InsertDataIntoParaglidingTicketStatusTable(long PurchasedTicketDetailId)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        //Find & get the User from existing users data
        private async Task<long> FindTheUserFromUsersData(string MobileNumber)
        {
            var user = _context.Users.Where(x => x.UserName == MobileNumber).FirstOrDefault();
            if (user != null)
                return user.UserId;
            else
                return 0;
        }

        #endregion
    }
}