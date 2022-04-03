using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Roles;
using System;
using System.Collections.Generic;
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

        protected readonly IMemoryCache _memoryCache;
        protected readonly DbContextModel _dbContext;
        public RoleController(IRoleService roleService, IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _roleService = roleService;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Models.Roles> result))
            {
                result = await _roleService.GetAll();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);
            }
            return Ok(result);
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
