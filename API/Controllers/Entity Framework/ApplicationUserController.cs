using Database;
using GlobalExceptionHandling.WebApi;
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
    [ApiController]
    public class ApplicationUserController : BaseController
    {
        private readonly IUserService _userSerivce;
        private readonly IMemoryCache _memoryCache;
        private readonly DbContextModel _dbContext;

        public ApplicationUserController(IUserService userSerivce, IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _userSerivce = userSerivce;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Policy ="Admin")]
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
        [Route("{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var result = await _userSerivce.GetById(userId);
            if (result == null)
                return Ok(result);

            return NotFound(new SomeException("User not found ", result));
        }

        //User/Recruiter/Admin can update their account
        [HttpPut]
        [Authorize(Policy ="AllAllowed")]
        public async Task<IActionResult> Update(UserRegistrationDTO req)
        {
            if (ModelState.IsValid)
            {
                var result = await _userSerivce.Update(UserId, req);
                if (result != null)
                    return Ok(new SomeException("Updated successfully.", result));

                return NotFound(new SomeException("An error occured.", result));
            }
            return BadRequest(ModelState);
        }

        // Only admin can delete Users/Recruiter/Admin
        [HttpDelete]
        [Authorize(Policy ="Admin")]
        [Route("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var checkUser = await _userSerivce.GetById(userId);
            if (checkUser != null)
            {
                var result = await _userSerivce.Delete(checkUser);
                if (result == true)
                    return Ok(new SomeException("Deleted successfully", result));
                return BadRequest(new SomeException("An errror occured"));
            }
            return NotFound(new SomeException($"User not found."));
        }
    }
}
