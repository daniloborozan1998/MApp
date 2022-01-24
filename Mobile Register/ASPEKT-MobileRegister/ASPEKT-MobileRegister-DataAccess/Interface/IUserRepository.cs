using ASPEKT_MobileRegister_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPEKT_MobileRegister_DataAccess.Interface
{
    public interface IUserRepository : IRepository<UserTest>
    {
        UserTest GetUserByEmail(string email);
        UserTest GetUserPhone(string phone);
        UserTest LoginUser(string email, string password);
    }
}
