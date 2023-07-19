using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserWallet.Controllers
{
    [Route("admin/users")]
    [Authorize(Roles = "Admin")]
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
        {
            return _userService.GetUsers();
        }

        [HttpPatch("block/{userId}")]
        public IActionResult BlockUser(string userId)
        {
            if (!int.TryParse(userId, out int id))
                return BadRequest();
            bool result = _userService.BlockUser(id);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpPatch("unblock/{userId}")]
        public IActionResult UnblockUser(string userId)
        {
            if (!int.TryParse(userId, out int id))
                return BadRequest();
            bool result = _userService.UnblockUser(id);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
