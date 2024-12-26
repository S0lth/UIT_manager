using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UITManagerApi.Models;

namespace UITManagerApi.Controllers {
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly JwtSettings _jwtSettings = new JwtSettings();
        private const string _relativePath = "registredUsers.json";
        private List<User> _allowedUsers = new List<User>();

        public AuthController(ILogger<AgentController> logger, JwtSettings jwtSettings) {
            _jwtSettings = jwtSettings;
            Load();
        }

        private void Load() {
            if (System.IO.File.Exists(_relativePath)) {
                string jsonString = System.IO.File.ReadAllText(_relativePath);
                _allowedUsers = JsonConvert.DeserializeObject<List<User>>(jsonString);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user) {
            if (_allowedUsers.IsNullOrEmpty()) Load();
            bool isSerialMatch = false;
            if (user != null) {
                isSerialMatch = _allowedUsers.Any(User => User.Serial == user.Serial);
            }
            return isSerialMatch ? Ok(generateToken()) : BadRequest();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult generateToken() {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
        
    }
}