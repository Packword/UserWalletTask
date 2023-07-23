using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserWallet.DTOs;
using UserWallet.Interfaces;
using UserWallet.Models;

namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("sign-up")]
        public IActionResult Add([FromBody] AddUserDTO userDto)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var result = _userService.AddUser(userDto.Username, userDto.Password);
            if (!result)
                return BadRequest();

            return Ok();
        }

        [HttpPost("login")]
        async public Task<IActionResult> Login([FromBody] UserDTO userDto)
        {
            User? user = _userService.GetUserByNameAndPassword(userDto.Username, userDto.Password);
            if (user is null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            
            return Ok(user);
        }

        [Authorize]
        [HttpPost("logout")]
        async public Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPatch("change-password")]
        [Authorize]
        public IActionResult ChangePassword(string newPassword)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            _userService.ChangePassword(newPassword, HttpContext);
            return Ok();
        }
    }
}
