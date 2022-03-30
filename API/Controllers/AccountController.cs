using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs;
using Services.OTPService;
using Services.Roles;
using Services.UserServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        
        public AccountController(IUserService userSerivce, IRoleService roleService, IConfiguration configuration, IOTPService oTPService)
        {
            _userSerivce = userSerivce;
            _roleSerivce = roleService;
            _configuration = configuration;
            _oTPService = oTPService;
        }

        [HttpPost]
        [Authorize(Policy ="Admin")]
        [Route("AdminRegistration")]
        public async Task<IActionResult> AddAdmin(Models.Users user)
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
        [Route("UserRegistration")]
        public async Task<IActionResult> UserRegistration(Models.Users user)
        {
            if (ModelState.IsValid)
            {
                bool checckEmail = await _userSerivce.CheckEmailIdExist(user.Email);
                if (checckEmail == false)
                {
                    await _userSerivce.AddUser(user);
                    return Ok();
                }
                return BadRequest("This email address is already taken. Please use another eamil address.");
            }
            return BadRequest("Model does not contains the attributes which are required.");
        }

        [HttpPost]
        [Route("RecruiterRegistration")]
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
                if(userDetails != null)
                {
                    var role = await _roleSerivce.GetById(userDetails.RoleId);
                    var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userDetails.UserName),
                        new Claim("UserId", Convert.ToString(userDetails.UserId), ClaimValueTypes.Integer),
                        new Claim("Email", userDetails.Email, ClaimValueTypes.String),
                        new Claim("RoleId", Convert.ToString(userDetails.RoleId), ClaimValueTypes.Integer),
                        new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                    var token = new JwtSecurityToken(
                        issuer:_configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        claims:claim,
                        expires: DateTime.Now.AddMonths(2),
                        signingCredentials: new SigningCredentials( securityKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                return Unauthorized();
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgeotPassword")]
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