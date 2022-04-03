﻿using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class AccountController : BaseController
    {
        protected readonly IUserService _userSerivce;
        protected readonly IRoleService _roleSerivce;
        protected readonly IConfiguration _configuration;
        protected readonly IOTPService _oTPService;
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
        [Route("Registration/Admin")]
        public async Task<IActionResult> AdminRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userSerivce.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userSerivce.AddAdmin(req);
                    return Ok(new SomeException("Saved", info));
                }

                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(new SomeException("Required fileds ", ModelState));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Registration/User")]
        public async Task<IActionResult> UserRegistration(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _userSerivce.CheckEmailIdExist(req.Email);
                if (checkEmail == false)
                {
                    var info = await _userSerivce.AddUser(req);
                    return Ok(new SomeException("Saved", info));
                }

                return NotFound(new SomeException("This email address is already taken." +
                    " Please use another eamil address.", req.Email));
            }
            return BadRequest(new SomeException("Required fileds ", ModelState));
        }

        [HttpPost]
        [Route("Registration/Recruiter")]
        public async Task<IActionResult> AddRecruiter(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                bool checckEmail = await _userSerivce.CheckEmailIdExist(req.Email);
                if (checckEmail == false)
                {
                    var info = await _userSerivce.AddRecruiter(req);
                    return Ok(new SomeException("Saved", info));
                }
                return BadRequest("This email address is already taken. Please use another eamil address.");
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
            return NotFound(new SomeException($"Emai does not exist {email}."));
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
                    return Ok(new SomeException("Password reset successfully.", result));

                return BadRequest(new SomeException("An error occured.", result));
            }
            return NotFound(new SomeException("Please enter OTP send to email.", getUser));
        }
    }
}