using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models.DTOs;
using Services.UserServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Users
{
    [Route("api/Users")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class ApplicationUserController : BaseController
    {
        protected readonly IUserService _userSerivce;
        protected readonly IMemoryCache _memoryCache;
        protected readonly DbContextModel _dbContext;

        public ApplicationUserController(IUserService userSerivce, IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _userSerivce = userSerivce;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Policy ="Admin")]
        //[Route("Users")]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Models.Users> result))
            {
                result = await _userSerivce.GetAll();
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
        [Authorize(Policy ="Admin")]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userSerivce.GetById(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPut]
        [Authorize(Policy ="AllAllowed")]
        [Route("{id}")]
        public async Task<IActionResult> Update(UserRegistrationDTO req)
        {
            var result = await _userSerivce.Update(UserId, req);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Policy ="Admin")]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userSerivce.Delete(id);
            if (result == false)
                return NotFound();

            return Ok(result);
        }
    }
}
