using ADOServices.ADOServices.RoleServices;
using Microsoft.AspNetCore.Mvc;
using Services.ADOServices.RoleServices;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "Admin")]
    public class ADORoleController : BaseController
    {
        //RolesServices _roleServices = new RolesServices();
        protected readonly IRoleServices _roleServices;
        public ADORoleController(IRoleServices roleServices)
        {
            _roleServices = roleServices;
        }

        [HttpGet]
        [Route("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _roleServices.GetAll());
        }

        [HttpGet]
        [Route("Roles/{id}")]
        public async Task<IActionResult> RoleById(int id)
        {
            return Ok(await _roleServices.GetById(id));
        }

        [HttpPost]
        [Route("Roles")]
        public async Task<IActionResult> AddRole(string role)
        {
            if (role != null)
                if (await _roleServices.Add(role, UserId))
                    return Ok();
            return BadRequest("The role should not be empty.");
        }

        [HttpPut]
        [Route("Roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, string role, bool isActive)
        {
            if (role != null)
                if (await _roleServices.Update(id, role, UserId, isActive))
                    return Ok();
            return BadRequest("The field should not be empty.");
        }

        [HttpDelete]
        [Route("Roles/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var checkId = await _roleServices.GetById(id);
            if(checkId != null)
                return Ok(await _roleServices.Delete(id));
            return NotFound();
        }
    }
}
