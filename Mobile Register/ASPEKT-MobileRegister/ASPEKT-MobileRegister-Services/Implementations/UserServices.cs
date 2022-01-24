using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ASPEKT_MobileRegister_DataAccess.Interface;
using ASPEKT_MobileRegister_Domain.Models;
using ASPEKT_MobileRegister_Dtos.Users;
using ASPEKT_MobileRegister_Mappers;
using ASPEKT_MobileRegister_Services.Interface;
using ASPEKT_MobileRegister_Shared;
using ASPEKT_MobileRegister_Shared.CustomException;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ASPEKT_MobileRegister_Services.Implementations
{
    public class UserServices : IUserServices
    {
        private IUserRepository _userRepository;
        IOptions<AppSettings> _options;

        public UserServices(IUserRepository userRepository, IOptions<AppSettings> options)
        {
            _userRepository = userRepository;
            _options = options;
        }

        public List<UserDtos> GetAllUsers()
        {
            List<UserTest> userDb = _userRepository.GetAll();
            return userDb.Select(x => x.ToUserDto()).ToList();
        }

        public UserDtos GetUserByID(int id)
        {
            UserTest userDb = _userRepository.GetById(id);
            if (userDb == null)
            {
                throw new ResourceNotFoundException($"UserTest with id {id} was not found");
            }

            return userDb.ToUserDto();
        }

        public void AddUsers(AddUpdateUserTestDto addUpdateUserTestDto)
        {
            ValidateUserUpdate(addUpdateUserTestDto);
            //use MD5 hash algorithm
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            //get the bytes from the password string
            byte[] passwordBytes = Encoding.ASCII.GetBytes(addUpdateUserTestDto.UserPassword);
            //get the hash
            byte[] passwordHash = mD5CryptoServiceProvider.ComputeHash(passwordBytes);
            //get the string hash
            string hashedPasword = Encoding.ASCII.GetString(passwordHash);

            UserTest newUser = new UserTest()
            {
                FirstName = addUpdateUserTestDto.UserFirstName,
                LastName = addUpdateUserTestDto.UserLastName,
                Email = addUpdateUserTestDto.UserEmail,
                Password = hashedPasword, //we send the hashed password value to the db
                PhoneNumber = addUpdateUserTestDto.UserPhoneNumber
            };
            _userRepository.Insert(newUser);
        }

        public void UpdateUsers(AddUpdateUserTestDto addUpdateUserTestDto)
        {
            UserTest userDb = _userRepository.GetById(addUpdateUserTestDto.UserTestId);
            if (userDb == null)
            {
                throw new ResourceNotFoundException(
                    $"UserTest with id {addUpdateUserTestDto.UserTestId} was not found");
            }
            //use MD5 hash algorithm
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            //get the bytes from the password string
            byte[] passwordBytes = Encoding.ASCII.GetBytes(addUpdateUserTestDto.UserPassword);
            //get the hash
            byte[] passwordHash = mD5CryptoServiceProvider.ComputeHash(passwordBytes);
            //get the string hash
            string hashedPasword = Encoding.ASCII.GetString(passwordHash);
            ValidateUserUpdate(addUpdateUserTestDto);
            userDb.FirstName = addUpdateUserTestDto.UserFirstName;
            userDb.LastName = addUpdateUserTestDto.UserLastName;
            userDb.Email = addUpdateUserTestDto.UserEmail;
            userDb.Password = hashedPasword;
            userDb.PhoneNumber = addUpdateUserTestDto.UserPhoneNumber;
            
            _userRepository.Update(userDb);

        }

        public void DeleteUsers(int id)
        {
            UserTest userDb = _userRepository.GetById(id);
            if (userDb == null)
            {
                throw new ResourceNotFoundException($"UserTest with {id} was not found");
            }
            _userRepository.Delete(userDb);
        }

        public void Register(RegisterUsersDto registerUsersDto)
        {
            ValidateUser(registerUsersDto);

            //use MD5 hash algorithm
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            //get the bytes from the password string
            byte[] passwordBytes = Encoding.ASCII.GetBytes(registerUsersDto.Password);
            //get the hash
            byte[] passwordHash = mD5CryptoServiceProvider.ComputeHash(passwordBytes);
            //get the string hash
            string hashedPasword = Encoding.ASCII.GetString(passwordHash);


            //Test123! - > 56945463 -> MD5 ->747534985 -> (hash) fe4rt

            UserTest newUser = new UserTest()
            {
                FirstName = registerUsersDto.FirstName,
                LastName = registerUsersDto.LastName,
                Email = registerUsersDto.Email,
                Password = hashedPasword,//we send the hashed password value to the db
                PhoneNumber = registerUsersDto.PhoneNumber
            };
            _userRepository.Insert(newUser);
        }

        public string Login(LoginDtos loginDtos)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] hashedBytes = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(loginDtos.Password));
            string hashedPassword = Encoding.ASCII.GetString(hashedBytes);

            UserTest userDb = _userRepository.LoginUser(loginDtos.Email, hashedPassword);
            if (userDb == null)
            {
                throw new ResourceNotFoundException($"Could not login user {loginDtos.Email}");
            }

            //GENERATE JWT TOKEN

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            byte[] secretKeyBytes = Encoding.ASCII.GetBytes(_options.Value.SecretKey);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(1), // the token will be valid for one hour
                //signature configuration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature),
                //payload
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, userDb.Email),
                        new Claim(ClaimTypes.NameIdentifier, userDb.IdUserTest.ToString()),
                        new Claim("userFullName", $"{userDb.FirstName} {userDb.LastName}")
                    }

                )

            };
            //generate token with the configuration
            SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            //convert it to string
            return jwtSecurityTokenHandler.WriteToken(token);
        }
        public void ValidateUser(RegisterUsersDto registerUserDto)
        {
            if (string.IsNullOrEmpty(registerUserDto.Email) || string.IsNullOrEmpty(registerUserDto.Password))
            {
                throw new UserException("Username and password are required fields!");
            }
            if (registerUserDto.Email.Length > 250)
            {
                throw new UserException("Email can contain maximum 250 characters!");
            }
            if (registerUserDto.FirstName.Length > 250 || registerUserDto.LastName.Length > 250)
            {
                throw new UserException("Firstname and Lastname can contain maximum 250 characters!");
            }
            if (!IsUserNameUnique(registerUserDto.Email))
            {
                throw new UserException("A user with this username already exists!");
            }
            if (registerUserDto.Password != registerUserDto.ConfirmedPassword)
            {
                throw new UserException("The passwords do not match!");
            }
            if (!IsPasswordValid(registerUserDto.Password))
            {
                throw new UserException("The password is not complex enough!");
            }
            if (!IsValidEmail(registerUserDto.Email))
            {
                throw new UserException("The email is not complex enough!");
            }
            if (!IsPhoneUnique(registerUserDto.PhoneNumber))
            {
                throw new UserException("A user with this phone number already exists!");
            }
            if (!IsPhoneNumber(registerUserDto.PhoneNumber))
            {
                throw new UserException("The phone number is not complex enough!(+38971254795)");
            }
        }
        public void ValidateUserUpdate(AddUpdateUserTestDto addUpdateUserTestDto)
        {
            if (string.IsNullOrEmpty(addUpdateUserTestDto.UserEmail) || string.IsNullOrEmpty(addUpdateUserTestDto.UserPassword))
            {
                throw new UserException("Email and password are required fields!");
            }
            if (addUpdateUserTestDto.UserEmail.Length > 250)
            {
                throw new UserException("Email can contain maximum 250 characters!");
            }

            if (!IsValidEmail(addUpdateUserTestDto.UserEmail))
            {
                throw new UserException("The email is not complex enough!");
            }
            if (addUpdateUserTestDto.UserFirstName.Length > 250 || addUpdateUserTestDto.UserLastName.Length > 250)
            {
                throw new UserException("Firstname and Lastname can contain maximum 250 characters!");
            }
            if (!IsUserNameUnique(addUpdateUserTestDto.UserEmail))
            {
                throw new UserException("A user with this email already exists!");
            }
            if (!IsPhoneUnique(addUpdateUserTestDto.UserPhoneNumber))
            {
                throw new UserException("A user with this phone number already exists!");
            }
            if (!IsPasswordValid(addUpdateUserTestDto.UserPassword))
            {
                throw new UserException("The password is not complex enough!");
            }
            if (!IsPhoneNumber(addUpdateUserTestDto.UserPhoneNumber))
            {
                throw new UserException("The phone number is not complex enough!(+38971254795)");
            }
        }


        private bool IsUserNameUnique(string email)
        {
            return _userRepository.GetUserByEmail(email) == null;
        }
        private bool IsPhoneUnique(string phone)
        {
            return _userRepository.GetUserPhone(phone) == null;
        }

        private bool IsPhoneNumber(string number)
        {
            return Regex.IsMatch(number, @"^(\+\d{1,3}[- ]?)?\d{10}$");
        }

        private bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        private bool IsPasswordValid(string password)
        {
            Regex passwordRegex = new Regex("^(?=.*[0-9])(?=.*[a-z]).{6,20}$");
            return passwordRegex.Match(password).Success;
        }
    }
}
