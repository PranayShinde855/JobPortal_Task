using ADOServices.ADOServices.UserServices;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADOApplicantionUserController : BaseController
    {
        protected readonly IUserServices _userServices;
        public ADOApplicantionUserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        //[Authorize(Policy = "Admin")]
        [Route("Users")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userServices.GetAll());
        }

        [HttpGet]
        //[Authorize(Policy = "Admin")]
        [Route("Users/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userServices.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPut]
        //[Authorize(Policy = "AllAllowed")]
        [Route("Users/{id}")]
        public async Task<IActionResult> Update(int id, UserRegistrationDTO user)
        {
            if (ModelState.IsValid)
            {
                var checkUser = await _userServices.GetById(id);
                if (checkUser != null)
                {
                    var result = await _userServices.Update(id, user);
                    if (result == false)
                        return BadRequest("Not Saved");

                    return Ok(result);
                }
                return NotFound();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        //[Authorize(Policy = "Admin")]
        [Route("Users/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var checkUser = await _userServices.GetById(id);
            if (checkUser != null)
            {
                var result = await _userServices.Delete(id);
                if (result == false)
                    return NotFound();

                return Ok(result);
            }
            return NotFound();
        }
    }
}
