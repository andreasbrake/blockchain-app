using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

using BlockchainAppAPI.Models.Authentication;

using BlockchainAppAPI.Logic.Authentication;
using BlockchainAppAPI.Logic.Utility;

namespace BlockchainAppAPI.Controllers.Authentication
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _currentUser;

        public LoginController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _config = configuration;
            _currentUser = httpContextAccessor.CurrentUser();
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return await Task.FromResult<IActionResult>(Ok("Logged in"));
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Bad State" });
            }
            
            // TODO: Replace with actual lookup
            UserModel user = await new UserManager().FindByUserNameAsync(model.UserName);
            
            if (user == null)
            {
                return BadRequest(new { error = "User does not exist" });
            }

            if (!SaltHash.VerifyHash(user.PasswordHash, model.Password))
            {
                return BadRequest(new { error = "Invalid password" });
            }
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return Ok(new
            { 
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            });
        }
    }
}
