using UserWallet.Services.Extensions;

namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserBalanceService _userBalanceService;

        public AuthController(IUserService userService, IUserBalanceService userBalanceService)
        {
            _userService = userService;
            _userBalanceService = userBalanceService;
        }

        [HttpPost("sign-up")]
        public IActionResult Add([FromBody] SignInDTO userDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            var result = _userService.AddUser(userDto.Username, userDto.Password);
            if (!result)
                return BadRequest("The user already exists");

            return Ok();
        }

        [HttpPost("login")]
        async public Task<IActionResult> Login([FromBody] LoginDTO userDto)
        {
            User? user = _userService.GetUserByNameAndPassword(userDto.Username, userDto.Password);
            if (user is null)
                return Unauthorized();

            user.Balances = _userBalanceService.GetUserBalances(user.Id);

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
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            _userService.ChangePassword(HttpContext.GetCurrentUserId(), newPassword);
            return Ok();
        }
    }
}
