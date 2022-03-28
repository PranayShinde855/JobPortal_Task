using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers.Roles
{
    [Route("api/Roles")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Authorize("Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Route("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _roleService.GetAll());
        }

        [HttpGet]
        [Route("GetRoleById")]
        public async Task<IActionResult> GetRoleById(int Id)
        {
            if (Id != null)
            {
                return Ok(await _roleService.GetById(Id));
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> AddRole(Models.Roles role)
        {
            if(ModelState.IsValid)
            return Ok(_roleService.Add(role));

            return BadRequest();
        }

        [HttpPut]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole(int Id, Models.Roles role)
        {
            if(Id != null)
                return Ok(_roleService.Update(Id, role));

            return BadRequest();
        }

        [HttpDelete]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole(int Id)
        {
            if (Id != null)
                return Ok( await _roleService.Delete(Id));

            return BadRequest();
        }
    }
}
