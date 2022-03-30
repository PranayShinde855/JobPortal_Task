using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.UserServices;
using System.Threading.Tasks;

namespace API.Controllers.Users
{
    [Route("api/[controller]")]
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
        [Route("Users")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userSerivce.GetAll());
        }

        [HttpGet]
        [Authorize(Policy ="Admin")]
        [Route("Users/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userSerivce.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPut]
        [Authorize(Policy ="AllAllowed")]
        [Route("Users/{id}")]
        public async Task<IActionResult> Update(int id, Models.Users user)
        {
            var result = await _userSerivce.Update(id, user);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Policy ="Admin")]
        [Route("Users/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userSerivce.Delete(id);
            if (result == false)
                return NotFound();

            return Ok(result);
        }
    }
}
