using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Services.OTPService;
using Services.Roles;
using Services.UserServices;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IUserService _userSerivce;
        private readonly IRoleService _roleSerivce;
        private readonly IConfiguration _configuration;
        private readonly IOTPService _oTPService;
        private readonly ILogger<AccountController> _logger;


        public AccountController(IUserService userSerivce, IRoleService roleService, 
            IConfiguration configuration, IOTPService oTPService, ILogger<AccountController> logger)
        {
            _userSerivce = userSerivce;
            _roleSerivce = roleService;
            _configuration = configuration;
            _oTPService = oTPService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy ="Admin")]
        [Route("Admin")]
        public async Task<IActionResult> AdminRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userSerivce.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userSerivce.AddAdmin(req, UserId);
                    if(info != null)
                        return Ok(new SomeException("Saved", info));

                    return BadRequest(new SomeException("An error occured."));
                }
                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Authorize("Admin")]
        [Route("Recruiter")]
        public async Task<IActionResult> AddRecruiter(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                bool checckEmail = await _userSerivce.CheckEmailIdExist(req.Email);
                if (checckEmail == false)
                {
                    var info = await _userSerivce.AddRecruiter(req, UserId);
                    if (info != null)
                        return Ok(new SomeException("Saved", info));

                    return BadRequest(new SomeException("An error occured."));
                }
                return BadRequest("This email address is already taken." +
                    " Please use another eamil address.");
            }
            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("User")]
        public async Task<IActionResult> UserRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userSerivce.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userSerivce.AddUser(req);
                    if (info != null)
                        return Ok(new SomeException("Saved", info));

                    return BadRequest(new SomeException("An error occured."));
                }

                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(ModelState);
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
                return NotFound(new SomeException("UserName or Password is invalid.", email, password)); ;
            }
            return BadRequest(new SomeException("Please enter UserName and Password.", ModelState));
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
                    return Ok(new SomeException("OTP send to registered email.", sendMail));

                return BadRequest(new SomeException("An error occured", sendMail));
            }
            return NotFound(new SomeException($"{email} Emai does not exist."));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO req)
        {
            if (req.Password == req.ConfirmPassword)
            {
                var checkEmail = await _userSerivce.GetUser(req.Email);
                if (checkEmail != null)
                {
                    var getUser = await _oTPService.GetByOTP(req.OTP, checkEmail.UserId);
                    if (getUser != null)
                    {
                        var result = await _userSerivce.ResetPassword(req, getUser.UserId);
                        if (result == true)
                            return Ok(new SomeException("Password reset successfully.", result));

                        return BadRequest(new SomeException("An error occured.", result));
                    }
                    return NotFound(new SomeException("Invalid OTP OR Email.", getUser));
                }
                return NotFound(new SomeException($"{req.Email} Email does not exist."));
            }
            return BadRequest(new SomeException("Password and Confirm Password should be same."));
        }
    }
}