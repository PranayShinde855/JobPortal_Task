using Database;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Services.ADOServices.RoleServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/ADO/Roles")]
    //[EnableCors("AllowOrigin")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class ADORoleController : BaseController
    {
        private readonly IRoleServices _roleServices;
        private readonly ILogger<ADORoleController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly DbContextModel _dbContext;

        public ADORoleController(IRoleServices roleServices, ILogger<ADORoleController> logger,
            IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _roleServices = roleServices;
            _logger = logger;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]
        //[Route("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Models.Roles> result))
            {
                result = await _roleServices.GetAll();
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
        [Route("{id}")]
        public async Task<IActionResult> RoleById(int id)
        {
            var result = await _roleServices.GetById(id);
            if(result != null)
                return Ok(result);

            return NotFound(new SomeException($"Role not found {id} "));
        }

        [HttpPost]
        //[Route("Roles")]
        public async Task<IActionResult> AddRole(string role)
        {
            if (role != null)
            {
                var result = await _roleServices.Add(role, UserId);
                if(result == true)
                    return Ok(new SomeException("Role added successfully."));
                return BadRequest(new SomeException("An error occured", false));
            }
            return BadRequest("Please enter role.");
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRole(int id, string role, bool isActive)
        {
            if (role != null)
            {
                var result = await _roleServices.Update(id, role, UserId, isActive);
                if (result == true)
                    return Ok(new SomeException($"Updated successfully {role}"));
                return BadRequest(new SomeException("An error occured", result));
            }
            return BadRequest("Please fill the details.");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var checkId = await _roleServices.GetById(id);
            if (checkId != null)
            {
                var result = await _roleServices.Delete(id);
                if (result == true)
                    return Ok(new SomeException($"Roles deleted successfully {id}"));
            }
            return NotFound(new SomeException($"Role not found {id}"));
        }
    }
}
