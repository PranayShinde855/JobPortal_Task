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
    [Route("api/Roles")]
    [ApiController]
    [Authorize(Policy ="Admin")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        private readonly IMemoryCache _memoryCache;
        private readonly DbContextModel _dbContext;
        public RoleController(IRoleService roleService, IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _roleService = roleService;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]        
        public async Task<IActionResult> GetRoles()
        {
            var cacheKey = string.Empty;
            object isExist = null;
            IEnumerable<Models.Roles> result = null;
            if (UserId != 0)
            {
                cacheKey = "Users_" + UserId.ToString();
                isExist = _memoryCache.Get(cacheKey);
            }

            if (isExist == null)
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
            else
            {
                result = (IEnumerable<Models.Roles>)isExist;
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            return Ok(await _roleService.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string role)
        {
            if(ModelState.IsValid)
            return Ok(await _roleService.Add(role, UserId));

            return BadRequest();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRole(int id, string role, bool isActive)
        {
            return Ok(await _roleService.Update(id, role, UserId, isActive));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            return Ok( await _roleService.Delete(id));
        }
    }
}
