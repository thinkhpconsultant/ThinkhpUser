using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkhpUserAPI.Models.Helper
{
    public static class ConstantValues
    {
        //For authenticate
        public static string ER_Msg_User_Credential_Wrong = $"Credential is wrong! Please try again.";

        #region Common Message
        public const string SC_Msg_Ins = $"Data Saved Successfully!";
        #endregion

        #region User Registration
        public const string WR_Msg_User_Mobile_Exists = $"{{field}} is already registered";

        //UserRegistrationRequestModel Validation

        //Required Validation
        public const string TXT_User_Validation_Message_Required_User_Name = "The First Name is required.";
        public const string TXT_User_Validation_Message_Required_User_Last_Name = "The Last Name is required.";
        public const string TXT_User_Validation_Message_Required_User_Password = "Your password must be at least 5 characters long and contain at least 1 letter and 1 number";
        //The password must be 6 characters long and contain at least one symbol(!, @, #, etc).

        //RegularExpression Validation
        public const string TXT_User_Validation_Message_RegularExpression_Address = "Please enter valid address without extra space and quotation( \' and \" )";
        public const string TXT_User_Validation_Message_RegularExpression_Mobile = "Entered mobile format is not valid.";
        #endregion
    }
}

