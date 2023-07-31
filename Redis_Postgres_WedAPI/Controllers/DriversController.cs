using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Redis_Postgres_WedAPI.Data;
using Redis_Postgres_WedAPI.Models;
using Redis_Postgres_WedAPI.Services;
using System.Xml.Linq;

namespace Redis_Postgres_WedAPI.Controllers;
    [ApiController]
    [Route("[controller]")]
public class DriversController : ControllerBase
    {

        private readonly ILogger<DriversController> _logger;
        private readonly ICacheService  _cacheService;
        private readonly AppDbContext  _context;

        public DriversController(AppDbContext context,
            ICacheService cacheService,
            ILogger<DriversController> logger)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = context;
        }
    [HttpGet("drivers")]
    public async Task<IActionResult> Get()
    {
        //check cache data
        var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");
        if(cacheData != null && cacheData.Count()>0)
            return Ok(cacheData);
        cacheData = await _context.Drivers.ToListAsync();

        //set expiry time
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<IEnumerable<Driver>>("drivers",cacheData,expiryTime);

        return Ok(cacheData);

    }

    [HttpPost("AddDriver")]
    public async Task<IActionResult> Post(Driver value)
    {
        var addedObj = await _context.Drivers.AddAsync(value);
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<Driver>($"driver{value.Id}",addedObj.Entity,expiryTime);
        _context.SaveChangesAsync();

        return Ok(addedObj.Entity);
    }

    [HttpDelete("DeleteDriver")]
    public async Task<IActionResult> Delete(int id)
    {
        var exist =await _context.Drivers.FirstOrDefaultAsync(x=>x.Id== id);
        if(exist != null)
        {
            _context.Remove(exist);
            _cacheService.RemoveData($"driver{id}");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        return NotFound();
    }

}
