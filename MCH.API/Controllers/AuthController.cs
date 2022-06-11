using System;
using System.Linq;
using System.Threading.Tasks;
using MCH.API.Configuration;
using MCH.API.Models;
using MCH.Data.Entities;
using MCH.Utils.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MCH.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController: Controller
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly AuthSettings _authSettings;
        public AuthController(UserManager<UserEntity> userManager, ILogger<AuthController> logger, IOptions<AuthSettings> authSettings)
        {
            _logger = logger;
            _userManager = userManager;
            _authSettings = authSettings.Value;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var user = new UserEntity() {UserName = model.UserName};
            var registerResult = await _userManager.CreateAsync(user,model.Password);

            if (registerResult.Succeeded)
            {
                _logger.LogInformation("Created user with username: {0}", user.UserName);
                return Created(string.Empty, string.Empty);
            }
            _logger.LogError("Error while registering new user: {0}", registerResult.Errors.First().Description);
            return Problem(registerResult.Errors.First().Description, null, 500);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            var user = _userManager.Users.SingleOrDefault(user => user.UserName == model.UserName);

            if(user == null)
            {
                _logger.LogError("User with username: {0} not found", user?.UserName);
                return NotFound("User not found");
            }

            var loginResult = await _userManager.CheckPasswordAsync(user, model.Password);

            if (loginResult)
            {
                _logger.LogInformation("User with username: {0}  logIn", user.UserName);
                var token = JwtHelpers.GenerateJWT(_authSettings.Issuer, DateTime.Now.AddMinutes(_authSettings.TokenLifeTime.Minutes),
                    user, _authSettings.Secret);
                return Ok(new { jwt_token = token });
            }

            _logger.LogError("Failed login with username: {0}. Incorrect data", user.UserName);
            return Problem("Login or password incorrect", null, StatusCodes.Status401Unauthorized);
        }
        
        [HttpGet]
        [Route("RefreshToken")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RefreshToken()
        {
            var userid = User.Claims.FirstOrDefault(c => c.Properties.Any(p => p.Value == JwtRegisteredClaimNames.Sub))?.Value;
            if (int.TryParse(userid, out var id))
            {
                var user = _userManager.Users.SingleOrDefault(u => u.Id == id);
                if(user != null){
                    _logger.LogInformation("Token for user: {0}  refreshed", user.UserName);
                    var token = JwtHelpers.GenerateJWT(_authSettings.Issuer, DateTime.Now.AddMinutes(_authSettings.TokenLifeTime.Minutes),
                        user, _authSettings.Secret);
                    return Ok( new { jwt_token = token});
                }
                _logger.LogError("Failed attemp to refresh token for user: {0}", user?.UserName);
            }
            return Unauthorized();
        }
    }
}