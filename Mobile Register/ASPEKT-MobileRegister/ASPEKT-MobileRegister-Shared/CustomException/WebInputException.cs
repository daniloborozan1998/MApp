using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPEKT_MobileRegister_Shared.CustomException
{
    public class WebInputException : Exception
    {
        public WebInputException(string message) : base(message)
        {

        }
    }
}
