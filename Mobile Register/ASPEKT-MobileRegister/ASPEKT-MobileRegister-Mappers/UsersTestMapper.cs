using ASPEKT_MobileRegister_Domain.Models;
using ASPEKT_MobileRegister_Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPEKT_MobileRegister_Mappers
{
    public static class UsersTestMapper
    {
        public static UserDtos ToUserDto(this UserTest userTest)
        {
            return new UserDtos()
            {
                UserTestId = userTest.IdUserTest,
                UserNameTest = $"{userTest.FirstName} {userTest.LastName}",
                UserEmail = userTest.Email,
                UserPassword = userTest.Password,
                PhoneNumber = userTest.PhoneNumber
            };
        }

        public static UserTest ToUserTest(this AddUpdateUserTestDto addUpdateUserTestDto)
        {
            return new UserTest()
            {
                IdUserTest = addUpdateUserTestDto.UserTestId,
                FirstName = addUpdateUserTestDto.UserFirstName,
                LastName = addUpdateUserTestDto.UserLastName,
                Email = addUpdateUserTestDto.UserEmail,
                Password = addUpdateUserTestDto.UserPassword,
                PhoneNumber = addUpdateUserTestDto.UserPhoneNumber
            };
        }
    }
}
