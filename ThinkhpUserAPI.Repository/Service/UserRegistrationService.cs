using Microsoft.EntityFrameworkCore;
using ThinkhpUserAPI.Models.Helper;
using ThinkhpUserAPI.Models.Models;
using ThinkhpUserAPI.Models.RequestModels;
using ThinkhpUserAPI.Models.ResponseModels;
using ThinkhpUserAPI.Repository.Interface;
namespace ThinkhpUserAPI.Repository.Service
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ThinkHPUsersContext _context;

        public UserRegistrationService(ThinkHPUsersContext context)
        {
            _context = context;
        }
        public async Task<CommonApiResponseModel> UserRegistration(UserRegistrationRequestModel model)
        {
            try
            {
                var tempData = _context.Users.ToList();
                string encodedPassword = EncryptDecryptHelper.EncodePassword(model.Password);
                User newUser = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Password = encodedPassword,
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
                return new CommonApiResponseModel { StatusCode = 0, Data = "" };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}