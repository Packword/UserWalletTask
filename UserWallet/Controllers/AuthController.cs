using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserWallet.DTOs;
using UserWallet.Interfaces;
using UserWallet.Models;

namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<User>? users = _userService.GetUsers();
            if (users is not null)
                return Ok(users);
            return NotFound();
        }

        [HttpPost("sign-up")]
        public IActionResult Add([FromQuery] UserDTO userDto)
        {
            if(!ModelState.IsValid)
                return BadRequest("Wrong length of Username or password");

            var res = _userService.AddUser(userDto.Username, userDto.Password);
            if (!res.result)
                return BadRequest(res.msg);

            return Ok();
        }

        [HttpPost("login")]
        async public Task<IActionResult> Login([FromQuery] UserDTO userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Wrong length of Username or Password");

            User? user = _userService.GetUserByNameAndPassword(userDto.Username, userDto.Password);
            bool res = await _authService.Login(user, HttpContext);
            if (!res)
                return Unauthorized();

            return Ok();
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
                return BadRequest("Wrong length of Username or Password");

            if (newPassword.Length < 4 || newPassword.Length > 8)
                return BadRequest("New password is not valid");

            _userService.ChangePassword(newPassword, HttpContext);
            return Ok();
        }
    }
}
