using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using web_api.Data;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class ClientsController : Controller
    {
        private readonly MyDbContext _context;

        public ClientsController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _context.Clients.ToListAsync();
            return Ok(clients);
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] Client clientRequest)
        {
            clientRequest.id = Guid.NewGuid();
            clientRequest.role = "client";
            await _context.Clients.AddAsync(clientRequest);
            await _context.SaveChangesAsync();

            return Ok(clientRequest);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetClient([FromRoute] Guid id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.id == id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateClient([FromRoute] Guid id, Client updatedClientRequest)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            client.name = updatedClientRequest.name;
            client.email = updatedClientRequest.email;
            client.password = updatedClientRequest.password;
            client.role = updatedClientRequest.role;

            await _context.SaveChangesAsync();

            return Ok(client);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteClient([FromRoute] Guid id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        [HttpGet]
        [Route("/devices{id:Guid}")]
        public async Task<IActionResult> LinkedDevices([FromRoute] Guid id)
        {
            var links = await _context.Links.Where(
                x => x.client_id.Equals(id)).ToListAsync();

            List<Devices> devices = new List<Devices>();
            foreach (var link in links)
            {
                var device = await _context.Devices.FirstOrDefaultAsync(
                    x => x.id.Equals(link.device_id));
                devices.Add(device);
            }

            return Ok(devices);
        }

        [HttpPut]
        [Route("/devices/link{client_id:Guid}")]
        public async Task<IActionResult> LinkNewDevice([FromRoute] Guid client_id, Devices device)
        {
            if (client_id == Guid.Empty)
                return BadRequest("client id is null");
            else if (device.id == Guid.Empty)
                return BadRequest("device id is null");
            else
            {
                Link link = new()
                {
                    client_id = client_id,
                    device_id = device.id
                };
                try
                {
                    await _context.Links.AddAsync(link);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return Conflict("Device already linked!");
                }
                return Ok(link);
            }
        }
    }
}
