using ASPEKT_MobileRegister_Dtos.Users;
using ASPEKT_MobileRegister_Services.Interface;
using ASPEKT_MobileRegister_Shared.CustomException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;

namespace ASPEKT_MobileRegister.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserServices _usersService;
        private readonly IOtpServices _otpService;
        private readonly IMemoryCache _memoryCache;
        public UserController(IUserServices usersService, IOtpServices otpService, IMemoryCache memoryCache)
        {
            _usersService = usersService;
            _otpService = otpService;
            _memoryCache = memoryCache;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<UserDtos>> Get()
        {
            try
            {
                return _usersService.GetAllUsers();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred!");
            }
        }
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<UserDtos> GetById(int id)
        {
            try
            {
                return _usersService.GetUserByID(id);
            }
            catch (ResourceNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, ($"UserTest with id {id} was not found"));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred!");
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddTestUser([FromBody] AddUpdateUserTestDto addUpdateUserTest)
        {
            try
            {
                _usersService.AddUsers(addUpdateUserTest);
                return StatusCode(StatusCodes.Status201Created, "User created successfully");
            }
            catch (WebInputException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, ($"Wrong data for new user was sent!"));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred!");
            }
        }
        [Authorize]
        [HttpPut]
        public IActionResult UpdateTestUser([FromBody] AddUpdateUserTestDto addUpdateUserTest)
        {
            try
            {
                _usersService.UpdateUsers(addUpdateUserTest);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (ResourceNotFoundException e)
            {
                //log
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (WebInputException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, ($"Wrong data for new user was sent!"));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred!");
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid id value!");
                }
                _usersService.DeleteUsers(id);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (ResourceNotFoundException e)
            {
                //log
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (Exception e)
            {
                //log
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred!");
            }
        }
        static int counter = 0;
        static object lockObj = new object();

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUsersDto userEntity)
        {
            _usersService.ValidateUser(userEntity);
            try
            {
                UserOtpDto newUserOtpDto = new UserOtpDto();
                newUserOtpDto.UserEntity = userEntity;
                newUserOtpDto.MailOtp = _otpService.GenerateOtp();
                newUserOtpDto.PhoneOtp = _otpService.GenerateOtp();
                Console.WriteLine(newUserOtpDto.MailOtp);
                Console.WriteLine(newUserOtpDto.PhoneOtp);

                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddMinutes(5);

                _memoryCache.Set(newUserOtpDto.UserEntity.Email, newUserOtpDto, cacheItemPolicy.AbsoluteExpiration);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous] //the user that sends the request can be unauthenticated
        [HttpPost("validate")]
        public ActionResult<string> Validate(string Email, string otpEmail, string otpMobile)
        {
            lock (lockObj)
            {
                counter++;
            }

            if (counter > 2)
            {
                throw new Exception("Too many tries");
            }
            var entity = _memoryCache.Get<UserOtpDto>(Email);
            if (entity != null && entity.MailOtp == otpEmail && entity.PhoneOtp == otpMobile)
            {
                 _usersService.Register(entity.UserEntity);
                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [AllowAnonymous] //the user that sends the request can be unauthenticated
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginDtos loginDto)
        {
            try
            {
                string token = _usersService.Login(loginDto);
                return Ok(token);
            }
            catch (Exception e)
            {
                //log
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred!");
            }
        }
    }
}
