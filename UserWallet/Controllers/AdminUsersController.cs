namespace UserWallet.Controllers
{
    [Route("admin/users")]
    [Authorize(Roles = UsersRole.ADMIN)]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public List<User> Get()
            => _userService.GetUsers();

        [HttpPatch("block/{userId:int}")]
        public IActionResult BlockUser(int userId)
        {
            bool result = _userService.BlockUser(userId);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpPatch("unblock/{userId:int}")]
        public IActionResult UnblockUser(int userId)
        {
            bool result = _userService.UnblockUser(userId);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
