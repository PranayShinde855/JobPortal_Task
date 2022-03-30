using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.DTOs;
using Services.OTPService;
using Services.Roles;
using Services.UserServices;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class AccountController : BaseController
    {
        protected readonly IUserService _userSerivce;
        protected readonly IRoleService _roleSerivce;
        protected readonly IConfiguration _configuration;
        protected readonly IOTPService _oTPService;

        
        public AccountController(IUserService userSerivce, IRoleService roleService, 
            IConfiguration configuration, IOTPService oTPService)
        {
            _userSerivce = userSerivce;
            _roleSerivce = roleService;
            _configuration = configuration;
            _oTPService = oTPService;
        }

        [HttpPost]
        [Authorize(Policy ="Admin")]
        [Route("Registration/Admin")]
        public async Task<IActionResult> AddRegistration(Models.Users user)
        {
            if (ModelState.IsValid)
            {
                bool checckEmail = await _userSerivce.CheckEmailIdExist(user.Email);
                if (checckEmail == false)
                {
                    await _userSerivce.AddAdmin(user);
                    return Ok();
                }

                return BadRequest("This email address is already taken. Please use another eamil address.");
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Registration/User")]
        public async Task<IActionResult> UserRegistration(Models.Users user)
        {
            if (ModelState.IsValid)
            {
                bool checkEmail = await _userSerivce.CheckEmailIdExist(user.Email);
                if (checkEmail == false)
                {
                    await _userSerivce.AddUser(user);
                    return Ok();
                }
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("Registration/Recruiter")]
        public async Task<IActionResult> AddRecruiter(Models.Users user)
        {
            if (ModelState.IsValid)
            {
                bool checckEmail = await _userSerivce.CheckEmailIdExist(user.Email);
                if (checckEmail == false)
                {
                    await _userSerivce.AddRecruiter(user);
                    return Ok();
                }
                return BadRequest("This email address is already taken. Please use another eamil address.");
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (email != null && password != null)
            {
                var userDetails = await _userSerivce.GetUser(email, password);
                if(userDetails !=  null)
                {
                    var userRole = await _roleSerivce.GetById(userDetails.RoleId);
                    GenerateTokenRequestDTO obj = new GenerateTokenRequestDTO();
                    obj.UserId = userDetails.UserId;
                    obj.UserName = userDetails.UserName;
                    obj.Password = userDetails.Password;
                    obj.Email = userDetails.Email;
                    obj.RoleId = userDetails.RoleId;
                    obj.Role = userRole.Name;
                    return Ok(await _userSerivce.GenerateToken(obj));
                }
                return BadRequest("UserName or Password is invalid.");
            }
            return BadRequest("Please enter UserName and Password.");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var data = await _userSerivce.GetUser(email);
            if (data != null)
            {
                var sendMail = await _userSerivce.SendMail(data);
                if (sendMail == true)
                    return Ok(true);

                return BadRequest();
            }
        
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var getUser = await _oTPService.GetByOTP(dto.OTP);
            if(getUser != null)
            {
                var result = await _userSerivce.ResetPassword(dto, getUser.UserId);

                if (result == true)
                    return Ok();

                return BadRequest();
            }
            return NotFound();
        }
    }
}