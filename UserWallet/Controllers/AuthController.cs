namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IUserBalanceService _userBalanceService;

        public AuthController(IUserService userService, IUserBalanceService userBalanceService, IAuthService authService)
        {
            _userService = userService;
            _userBalanceService = userBalanceService;
            _authService = authService;
        }

        [HttpPost("sign-up")]
        public IActionResult Add([FromBody] SignUpDTO userDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            var result = _userService.AddUser(userDto.Username!, userDto.Password!);
            if (!result)
                return BadRequest("The user already exists");

            return Ok();
        }

        [HttpPost("login")]
        async public Task<IActionResult> Login([FromBody] LoginDTO userDto)
        {
            if (HttpContext.GetCurrentUserId() != -1)
                return BadRequest("The user is already logged in");

            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            User? user = _userService.GetUserByNameAndPassword(userDto.Username!, userDto.Password!);
            if (user is null)
                return Unauthorized();

            user.Balances = _userBalanceService.GetUserBalances(user.Id);
            var principal = _authService.GenerateClaims(user);
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
        public IActionResult ChangePassword([FromBody] ChangeUserPasswordDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            _userService.ChangePassword(HttpContext.GetCurrentUserId(), model.NewPassword!);
            return Ok();
        }
    }
}
