using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPEKT_MobileRegister_Dtos.Users
{
    public class UserOtpDto
    {
        public RegisterUsersDto UserEntity { get; set; }
        public string PhoneOtp { get; set; }
        public string MailOtp { get; set; }
    }
}
