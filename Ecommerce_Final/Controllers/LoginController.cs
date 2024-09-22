using Ecommerce_Final.Common;
using Ecommerce_Final.Data;
using Ecommerce_Final.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce_Application_VerveBridge_Internship_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public readonly ApplicationDbContext db;
        public readonly IConfiguration _config;

        public LoginController(ApplicationDbContext _db, IConfiguration config)
        {
            this.db = _db;
            _config = config;
        }
        // GET: api/<LoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public IActionResult Login([FromBody] LoginDto LoginRequest)
        {
            var userCredentials = db.Users.FirstOrDefault(u => u.Username == LoginRequest.username);
            var Username = userCredentials.Username;
            var Password = CommonMethods.ConvertToDecrypt(userCredentials.PasswordHash);

            if (LoginRequest.username == Username && LoginRequest.password == Password)
            {
                var userId = userCredentials.Id;
                var userRole = userCredentials.Role;
                var authClaims = new List<Claim>
                 {
                     new Claim("Username",Username),
                     new Claim("UserId",userId.ToString()),
                     new Claim(ClaimTypes.Role, userRole)
                 };
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:ValidIssuer"],
                    audience: _config["Jwt:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
