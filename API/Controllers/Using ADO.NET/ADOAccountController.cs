using ADOServices.ADOServices.OTPServices;
using ADOServices.ADOServices.UserServices;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Services.ADOServices.RoleServices;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/ADO/Account")]
    //[EnableCors("AllowOrigin")]
    [ApiController]
    public class ADOAccountController : BaseController
    {
        private readonly IUserServices _userServices;
        private readonly IOTPServices _oTPServices;
        private readonly IRoleServices _roleServices;
        private readonly ILogger<ADOAccountController> _logger;
        public ADOAccountController(IUserServices userServices, IOTPServices oTPServices, IRoleServices roleServices
            , ILogger<ADOAccountController> logger)
        {
            _userServices = userServices;
            _oTPServices = oTPServices;
            _roleServices = roleServices;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("Admin")]
        public async Task<IActionResult> AdminRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userServices.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userServices.AddAdmin(req, UserId);
                    if (info == true)
                        return Ok(new  SomeException ("Saved", req));
                }
                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Authorize("Admin")]
        [Route("Recruiter")]
        public async Task<IActionResult> RecruiterRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userServices.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userServices.AddRecruiter(req, UserId);
                    if (info == true)
                        return Ok(new SomeException("Saved", info));
                }
                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("User")]
        public async Task<IActionResult> UserRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userServices.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userServices.AddUser(req);
                    if (info == true)
                        return Ok(new SomeException("Saved", info));
                }
                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (email != null && password != null)
            {
                var getUser = await _userServices.GetUser(email, password);
                if(getUser !=null)
                {
                    var roleInfo = await _roleServices.GetById(getUser.RoleId);
                    GenerateTokenRequestDTO obj = new GenerateTokenRequestDTO();
                    obj.UserId = getUser.UserId;
                    obj.UserName = getUser.UserName;
                    obj.Password = getUser.Password;
                    obj.Email = getUser.Email;
                    obj.RoleId = getUser.RoleId;
                    obj.Role = roleInfo.Name;
                    return Ok(await _userServices.GenerateToken(obj));
                }
                return NotFound(new SomeException("UserName or Password is invalid.", email, password));
            }
            return BadRequest(new SomeException("Please enter UserName and Password.", ModelState));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var data = await _userServices.GetUser(email);
            if (data != null)
            {
                var sendMail = await _userServices.SendMail(data);
                if (sendMail == true)
                    return Ok(new SomeException("OTP send to registered email.", sendMail));

                return BadRequest(new SomeException("An error occured.", sendMail));
            }
            return BadRequest(new SomeException($"{email} Email does not exist."));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO req)
        {
            if (req.Password == req.ConfirmPassword)
            {
                var checkEmail = await _userServices.GetUser(req.Email);
                if (checkEmail != null)
                {
                    var getUser = await _oTPServices.GetByOTP(req.OTP);
                    if (getUser != null)
                    {
                        var result = await _userServices.ResetPassword(req, getUser.UserId);
                        if (result == true)
                            return Ok(new SomeException("Password reset successfully.", result));

                        return BadRequest(new SomeException("An error occured.", result));
                    }
                    return NotFound(new SomeException("Invalid OTP.", getUser));
                }
                return BadRequest(new SomeException($"{req.Email} does not exist."));

            }

            return BadRequest(new SomeException("Password and Confirm Password should be same."));
        }
    }
}
