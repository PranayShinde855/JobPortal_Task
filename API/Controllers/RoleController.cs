using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.Roles;
using System.Threading.Tasks;

namespace API.Controllers.Roles
{
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Authorize(Policy ="Admin")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Route("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _roleService.GetAll());
        }

        [HttpGet]
        [Route("Roles/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            return Ok(await _roleService.GetById(id));
        }

        [HttpPost]
        [Route("Roles")]
        public async Task<IActionResult> AddRole(Models.Roles role)
        {
            if(ModelState.IsValid)
            return Ok(await _roleService.Add(role));

            return BadRequest();
        }

        [HttpPut]
        [Route("Roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, Models.Roles role)
        {
            return Ok(await _roleService.Update(id, role));
        }

        [HttpDelete]
        [Route("Roles/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            return Ok( await _roleService.Delete(id));
        }
    }
}
