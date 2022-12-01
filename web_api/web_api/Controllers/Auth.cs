using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using web_api.Data;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Auth : Controller
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;

        public Auth(MyDbContext context, IConfiguration configuration)
        {
            _context = context;

            _configuration = configuration;
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> LogIn(Client client)
        {
            if (client == null)
            {
                return BadRequest("Client not found!");
            }

            var dbClient = await _context.Clients
                .FirstOrDefaultAsync(x => x.email == client.email &&
                x.password == client.password);
            
            if(dbClient == null)
            {
                return NotFound(client);
            }

            string token = CreateToken(dbClient);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, dbClient);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(Client client)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!client.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (client.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(client);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, client);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken,
            Client client)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            client.RefreshToken = newRefreshToken.Token;
            client.TokenCreated = newRefreshToken.Created;
            client.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(Client client)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, client.email),
                new Claim(ClaimTypes.Role, client.role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<string>> SignUp(Client client)
        {
            if (client == null)
            {
                return BadRequest();
            }

            var dbClient = await _context.Clients
                .FirstOrDefaultAsync(x => x.email == client.email);

            if (dbClient != null)
            {
                return BadRequest(new {Message = "Client already exists!"});
            }

            client.id = new Guid();
            client.role = "client";

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }
    }
}
