using ASPEKT_MobileRegister_Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPEKT_MobileRegister_Services.Interface
{
    public interface IUserServices
    {
        List<UserDtos> GetAllUsers();
        UserDtos GetUserByID(int id);
        void AddUsers(AddUpdateUserTestDto addUpdateUserTestDto);
        void UpdateUsers(AddUpdateUserTestDto addUpdateUserTestDto);
        void DeleteUsers(int id);
        void Register(RegisterUsersDto registerUsersDto);
        string Login(LoginDtos loginDtos);
        void ValidateUser(RegisterUsersDto registerUserDto);
    }
}
