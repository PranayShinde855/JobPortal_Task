using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.UserServices;
using System.Threading.Tasks;

namespace API.Controllers.Users
{
    [Route("api/UserController")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class ApplicationUserController : BaseController
    {
        protected readonly IUserService _userSerivce;
        public ApplicationUserController(IUserService userSerivce)
        {
            _userSerivce = userSerivce;
        }

        [HttpGet]
        [Authorize(Policy ="Admin")]
        [Route("UserList")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userSerivce.GetAll());
        }

        [HttpGet]
        [Authorize(Policy ="Admin")]
        [Route("GetUserById")]
        public async Task<IActionResult> GetById(int Id)
        {
            if (Id != null)
            {
                return Ok(await _userSerivce.GetById(Id));
            }
            return NotFound("UserId not found.");
        }


        [HttpPut]
        [Authorize(Policy ="AllAllowed")]
        [Route("UpdateUser")]
        public async Task<IActionResult> Update(int Id, Models.Users user)
        {
            if (Id != null)
            {
                return Ok(await _userSerivce.Update(Id, user));
            }
            return NotFound();
        }

        [HttpDelete]
        [Authorize(Policy ="Admin")]
        [Route("DeleteUser")]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id != null)
            {
                return Ok(await _userSerivce.Delete(Id));
            }
            return NotFound("UserId not found.");
        }
    }
}
