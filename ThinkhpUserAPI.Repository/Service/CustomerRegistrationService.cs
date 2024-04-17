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
            User newUser = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
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
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new CommonApiResponseModel { StatusCode = 0, Data = 0 };
        }
    }
}