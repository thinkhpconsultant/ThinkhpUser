using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkhpUserAPI.Models.RequestModels
{
    public class CustomerRegistrationRequestModel
    {
        public long UserId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? MobileNumber { get; set; }
        public string? AlternateMobileNumber { get; set; }
        public string? Email { get; set; }
        [Required]
        public int? CityId { get; set; }
        [Required]
        public int? StateId { get; set; }
        [Required]
        public int? PinCode { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? InsertedOn { get; set; }
        public long? InsertedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
