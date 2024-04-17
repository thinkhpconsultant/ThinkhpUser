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
        public async Task<CommonApiResponseModel> UserRegistration(UserRegistrationRequestModel userRegistrationRequestModel)
        {
            try
            {
                var userMobileNo = await _context.Users.AnyAsync(x => x.MobileNumber == userRegistrationRequestModel.MobileNumber);
                if (userMobileNo)
                    return new CommonApiResponseModel { StatusCode = 2, Message = ConstantValues.WR_Msg_User_Mobile_Exists.Replace("{field}", "User with this mobile no.") };

                var userData = _context.Users.ToList();
                string encodedPassword = EncryptDecryptHelper.EncodePassword(userRegistrationRequestModel.Password);
                User newUser = new()
                {
                    FirstName = userRegistrationRequestModel.FirstName,
                    LastName = userRegistrationRequestModel.LastName,
                    UserName = userRegistrationRequestModel.MobileNumber,
                    Password = encodedPassword,
                    Address = userRegistrationRequestModel.Address,
                    MobileNumber = userRegistrationRequestModel.MobileNumber,
                    AlternateMobileNumber = userRegistrationRequestModel.AlternateMobileNumber,
                    IsDeleted = false,
                    InsertedOn = DateTime.UtcNow,
                };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return new CommonApiResponseModel { StatusCode = 0, Message = ConstantValues.SC_Msg_Ins, Data = newUser.UserId };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}