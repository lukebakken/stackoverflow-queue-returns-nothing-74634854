using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using web_api.Data;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : Controller
    {
        private readonly MyDbContext _context;

        public DeviceController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await _context.Devices.ToListAsync();
            return Ok(devices);
        }

        [HttpPost]
        public async Task<IActionResult> AddDevice([FromBody] Devices deviceReqest)
        {
            deviceReqest.id = new Guid();

            await _context.Devices.AddAsync(deviceReqest);
            await _context.SaveChangesAsync();

            return Ok(deviceReqest);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetClient([FromRoute] Guid id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(x => x.id == id);

            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateClient([FromRoute] Guid id, Devices updatedDeviceRequest)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            device.description = updatedDeviceRequest.description;
            device.address = updatedDeviceRequest.address;
            device.max_consumption = updatedDeviceRequest.max_consumption;

            await _context.SaveChangesAsync();

            return Ok(device);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteClient([FromRoute] Guid id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
                return NotFound();

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return Ok(device);
        }
    }
}
