using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkhpUserAPI.Models.Helper;
using ThinkhpUserAPI.Models.ResponseModels;

namespace ThinkhpUserAPI.Models.RequestModels
{
    public class ParaglidingTicketPurchaseRequestModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternateMobileNumber { get; set; }
        public string? Email { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? PinCode { get; set; }
        public bool? IsDeleted { get; set; }
        public List<CustomerRegistrationRequestModel>? customerList { get; set; }
    }
}
