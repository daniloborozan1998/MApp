using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASPEKT_MobileRegister_DataAccess;
using ASPEKT_MobileRegister_DataAccess.Implementations;
using ASPEKT_MobileRegister_DataAccess.Interface;
using ASPEKT_MobileRegister_Services.Implementations;
using ASPEKT_MobileRegister_Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASPEKT_MobileRegister_Helpers
{
    public static class DependencyInjectionHelper
    {
        public static void InjectDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ASPEKTDbContext>(x =>
                x.UseSqlServer(connectionString));
        }
        public static void InjectRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>(); //DI
            
        }
        public static void InjectServices(IServiceCollection services)
        {
            services.AddTransient<IUserServices, UserServices>();
            services.AddTransient<IOtpServices, OtpService>(); //DI//DI
        }
    }
}
