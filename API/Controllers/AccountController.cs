using Database.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs;
using Services;
using Services.OTPService;
using Services.Roles;
using Services.UserServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/Account")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        protected readonly IUserSerivce _userSerivce;
        protected readonly IRoleService _roleSerivce;
        protected readonly IConfiguration _configuration;
        protected readonly IOTPService _oTPService;

        
        public AccountController(IUserSerivce userSerivce, IRoleService roleService, IConfiguration configuration, IOTPService oTPService)
        {
            _userSerivce = userSerivce;
            _roleSerivce = roleService;
            _configuration = configuration;
            _oTPService = oTPService;
        }

        [HttpPost]
        [Route("UserRegistration")]
        public async Task<IActionResult> AddUser(Models.Users user)
        {
            if (ModelState.IsValid)
            {
                await _userSerivce.AddUser(user);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("RecruterRegistration")]
        public async Task<IActionResult> AddRecruter(Models.Users user)
        {
            if (ModelState.IsValid)
            {
                await _userSerivce.AddUser(user);
                return Ok();
            }
            return BadRequest();
        }

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

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var token = new JwtSecurityToken(
                        issuer:_configuration["JWT:ValidAudeince"],
                        audience: _configuration["JWT:ValidIssuer"],
                        claims:claim,
                        expires: DateTime.Now.AddMinutes(60),
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

        [HttpPost]
        [Route("ForgeotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var data = await _userSerivce.GetUser(email);
            if (data != null)
            {
                var otp = await _oTPService.GenerateOTP(data.UserId);
                var sub = "OTP";
                var body = "";
                body += "<h3>Job Portal</h3>";
                body += $"<h4 style='font-size:1.1em'>Hi, {data.UserName}</h4>";
                body += $"<h4 style='font-size:1.1em'>This is your OTP to reset your password.</h4>";
                body += $"<h2 style='background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;'>{otp}</h2>";

                var sendMail = await _userSerivce.SendEmail(data.Email, sub, body);
                if (sendMail == true)
                    return Ok(true);

                return BadRequest();
            }
        
            return BadRequest();
        }

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