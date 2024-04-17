using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkhpUserAPI.Models.Helper;

namespace ThinkhpUserAPI.Models.RequestModels
{
    public class CustomerRegistrationRequestModel
    {
        public long UserId { get; set; }
        [Required(ErrorMessage = @ConstantValues.TXT_User_Validation_Message_Required_User_Name)]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = @ConstantValues.TXT_User_Validation_Message_Required_User_Last_Name)]
        public string? LastName { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{5,}", ErrorMessage = @ConstantValues.TXT_User_Validation_Message_Required_User_Password)]
        public string? Password { get; set; }
        [Required]
        [RegularExpression(@"^[^\s'""].*?(?<!\s)$", ErrorMessage = @ConstantValues.TXT_User_Validation_Message_RegularExpression_Address)]
        [StringLength(150)]
        public string? Address { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = @ConstantValues.TXT_User_Validation_Message_RegularExpression_Mobile)]
        public string? MobileNumber { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = @ConstantValues.TXT_User_Validation_Message_RegularExpression_Mobile)]
        public string? AlternateMobileNumber { get; set; }
        public string? Email { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? PinCode { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? InsertedOn { get; set; }
        public long? InsertedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
