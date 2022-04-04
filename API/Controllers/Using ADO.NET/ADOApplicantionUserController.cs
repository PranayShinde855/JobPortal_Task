using ADOServices.ADOServices.UserServices;
using Database;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/ADO/Users")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class ADOApplicantionUserController : BaseController
    {
        protected readonly IUserServices _userServices;
        protected readonly ILogger<ADOApplicantionUserController> _logger;
        protected readonly IMemoryCache _memoryCache;
        protected readonly DbContextModel _dbContext;
        
        public ADOApplicantionUserController(IUserServices userServices, ILogger<ADOApplicantionUserController> logger,
            IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _userServices = userServices;
            _logger = logger;
            _memoryCache = memoryCache;
            _dbContext = dbContext; 
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        //[Route("Users")]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Models.Users> result))
            {
                result = await _userServices.GetAll();
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
        [Authorize(Policy = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userServices.GetById(id);
            if (result != null)
                return Ok(result);
            return NotFound(new SomeException($"User not found {result}"));
        }


        [HttpPut]
        [Authorize(Policy = "AllAllowed")]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, UserRegistrationDTO user)
        {
            if (ModelState.IsValid)
            {
                var checkUser = await _userServices.GetById(id);
                if (checkUser != null)
                {
                    var result = await _userServices.Update(id, user);
                    if (result == false)
                        return BadRequest(new SomeException("An error occured.", result));

                    return Ok(new SomeException("Updated successfully.", result));
                }
                return NotFound(new SomeException("User not found {checkUser}"));
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Authorize(Policy = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var checkUser = await _userServices.GetById(id);
            if (checkUser != null)
            {
                var result = await _userServices.Delete(id);
                if (result == false)
                    return NotFound(new SomeException("An error occured" , checkUser));

                return Ok(new SomeException($"Deleted successfully {result}"));
            }
            return NotFound(new SomeException($"User not found {checkUser}"));
        }
    }
}