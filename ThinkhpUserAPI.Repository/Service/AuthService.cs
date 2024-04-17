using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    public class AuthService : IAuthService
    {
        private readonly ThinkHPUsersContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ThinkHPUsersContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<CommonApiResponseModel> Login(AuthenticateRequestModel model)
        {
            string encodedPassword = EncryptDecryptHelper.EncodePassword(model.Password);
            User? userDet = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.Username && u.Password == encodedPassword);
            if (userDet == null) //|| BCrypt.Verify(password, userDet.Password) == false
                return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.ER_Msg_User_Credential_Wrong };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_configuration["JWT:SecretKey"]));
            
            var sessionTimeOut = _configuration.GetSection("SessionTimeOut");
            double sessionTimeOutMinutes = Convert.ToDouble(string.IsNullOrEmpty(sessionTimeOut.Value) ? 60 : sessionTimeOut.Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userDet.UserName),
                    new Claim(ClaimTypes.GivenName, userDet.FirstName),
                    //new Claim(ClaimTypes.Role, user.Role)
                }),
                IssuedAt = DateTime.UtcNow,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(sessionTimeOutMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey.Key), SecurityAlgorithms.HmacSha256Signature),
        };


            try
            {
                var tokenDesc = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(tokenDesc);

                UserLogInToken objLoginToken = new()
                {
                    UserId = userDet.UserId,
                    Token = token,
                    TokenExpireTime = tokenDescriptor.Expires,
                    InsertedOn = DateTime.UtcNow,
                    InsertedBy = 1,
                    IsUsed = false
                };

                await _context.AddAsync(objLoginToken);
                await _context.SaveChangesAsync();

                AuthenticateResponseModel resObj = new()
                {
                    UserId = userDet.UserId,
                    Name = $"{userDet.FirstName} {userDet.LastName}",
                    Username = userDet.UserName,
                    Token = token,
                    TokenExpireTime = Convert.ToString(objLoginToken.TokenExpireTime),
                };
                return new CommonApiResponseModel { StatusCode = 0, Data = resObj };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception during token creation: {ex.Message}");
                throw; // Rethrow the exception to see details in the debugger
            }
        }
        public async Task<CommonApiResponseModel> Logout(LogoutRequestModel model)
        {
            var userDet = await _context.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);
            if (userDet == null)
                return new CommonApiResponseModel { StatusCode = 1, Message = ConstantValues.ER_Msg_User_Credential_Wrong };
            else
            {
                userDet.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            return new CommonApiResponseModel { StatusCode = 0, Message = "Logout Successful" };
        }
    }
}