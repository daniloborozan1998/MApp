using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASPEKT_MobileRegister_DataAccess.Interface;
using ASPEKT_MobileRegister_Domain.Models;

namespace ASPEKT_MobileRegister_DataAccess.Implementations
{
    public class UserRepository : IUserRepository
    {
        private ASPEKTDbContext _aspektDbContext;

        public UserRepository(ASPEKTDbContext aspektDbContext)
        {
            _aspektDbContext = aspektDbContext;
        }
        public List<UserTest> GetAll()
        {
            return _aspektDbContext.UserTestMobile.ToList();
        }

        public UserTest GetById(int id)
        {
            return _aspektDbContext
                .UserTestMobile.FirstOrDefault(x => x.IdUserTest == id);
        }

        public void Insert(UserTest entity)
        {
            _aspektDbContext.UserTestMobile.Add(entity);
            _aspektDbContext.SaveChanges();
        }

        public void Update(UserTest entity)
        {
            _aspektDbContext.UserTestMobile.Update(entity);
            _aspektDbContext.SaveChanges();
        }

        public void Delete(UserTest entity)
        {
            _aspektDbContext.UserTestMobile.Remove(entity);
            _aspektDbContext.SaveChanges();
        }

        public UserTest GetUserByEmail(string email)
        {
            return _aspektDbContext.UserTestMobile.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
        }

        public UserTest GetUserPhone(string phone)
        {
            return _aspektDbContext.UserTestMobile.FirstOrDefault(x => x.PhoneNumber.ToLower() == phone.ToLower());
        }

        public UserTest LoginUser(string email, string password)
        {
            return _aspektDbContext.UserTestMobile.FirstOrDefault(x => x.Email.ToLower() == email.ToLower()
                                                                  && x.Password == password);
        }
    }
}
