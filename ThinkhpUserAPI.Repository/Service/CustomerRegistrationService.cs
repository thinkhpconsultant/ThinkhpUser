using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ThinkhpUserAPI.Models.Helper;
using ThinkhpUserAPI.Models.Models;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;
using ThinkhpUserAPI.Repository.Interface;

namespace ThinkhpUserAPI.Repository.Service
{
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly ThinkHPUsersContext _context;
        private readonly IConfiguration _configuration;

        public CustomerRegistrationService(ThinkHPUsersContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<CommonApiResponseModel> CustomerRegistration(CustomerRegistrationRequestModel model)
        {
            var customerMobileNo = await _context.Users.AnyAsync(x => x.MobileNumber == model.MobileNumber);
            if (customerMobileNo)
                return new CommonApiResponseModel { StatusCode = 2, Message = ConstantValues.WR_Msg_User_Mobile_Exists.Replace("{field}", "User with this mobile no.") };
            string encodedPassword = EncryptDecryptHelper.EncodePassword(model.Password);

            User newCustomer = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                MobileNumber = model.MobileNumber,
                AlternateMobileNumber = model.AlternateMobileNumber,
                Email = model.Email,
                CityId = model.CityId,
                StateId = model.StateId,
                PinCode = model.PinCode,
                IsDeleted = false,
                InsertedBy = 1,
                InsertedOn = DateTime.UtcNow,
            };

            if (!string.IsNullOrEmpty(model.Password))
            {
                newCustomer.UserName = model.MobileNumber;
                newCustomer.Password = encodedPassword;
            }
            await _context.Users.AddAsync(newCustomer);
            await _context.SaveChangesAsync();

            return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.SC_Msg_Ins, Data = newCustomer.UserId };
        }
    }
}