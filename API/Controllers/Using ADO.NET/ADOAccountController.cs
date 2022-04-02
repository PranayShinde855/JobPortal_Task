using ADOServices.ADOServices.OTPServices;
using ADOServices.ADOServices.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.ADOServices.RoleServices;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADOAccountController : ControllerBase
    {
        public readonly IUserServices _userServices;
        public readonly IOTPServices _oTPServices;
        public readonly IRoleServices _roleServices;
        public ADOAccountController(IUserServices userServices, IOTPServices oTPServices, IRoleServices roleServices)
        {
            _userServices = userServices;
            _oTPServices = oTPServices;
            _roleServices = roleServices;
        }

        [HttpPost]
        //[Authorize(Policy = "Admin")]
        [Route("Registration/Admin")]
        public async Task<IActionResult> AdminRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userServices.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userServices.AddAdmin(req);
                    if (info == true)
                        return Ok(true);
                }
                return BadRequest("This email address is already taken. Please use another eamil address.");
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        //[AllowAnonymous]
        [Route("Registration/Recruiter")]
        public async Task<IActionResult> RecruiterRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userServices.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userServices.AddRecruiter(req);
                    if (info == true)
                        return Ok(true);
                }
                return BadRequest("This email address is already taken. Please use another eamil address.");
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        //[AllowAnonymous]
        [Route("Registration/User")]
        public async Task<IActionResult> UserRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userServices.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userServices.AddUser(req);
                    if (info == true)
                        return Ok(true);
                }
                return BadRequest("This email address is already taken. Please use another eamil address.");
            }
            return BadRequest(ModelState); ;
        }

        [AllowAnonymous]
        [HttpPost]
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
                return BadRequest("UserName or Password is invalid.");
            }
            return BadRequest("Please enter UserName and Password.");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var data = await _userServices.GetUser(email);
            if (data != null)
            {
                var sendMail = await _userServices.SendMail(data);
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
            var getUser = await _oTPServices.GetByOTP(dto.OTP);
            if (getUser != null)
            {
                var result = await _userServices.ResetPassword(dto, getUser.UserId);

                if (result == true)
                    return Ok();

                return BadRequest();
            }
            return NotFound();
        }
    }
}
