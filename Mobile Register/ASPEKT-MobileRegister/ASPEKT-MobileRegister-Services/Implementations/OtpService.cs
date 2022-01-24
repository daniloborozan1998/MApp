using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASPEKT_MobileRegister_Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace ASPEKT_MobileRegister_Services.Implementations
{
    public class OtpService : IOtpServices
    {
        private readonly IMemoryCache _memoryCache;


        public OtpService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string GenerateOtp()
        {

            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");
            return code;

        }
    }
}
