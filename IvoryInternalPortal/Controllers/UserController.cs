using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IvoryInternalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userRepo;

        public UserController(IUser userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] Users user)
        {
            try
            {
                if (user == null)
                    return BadRequest(new { message = "User data is required" });

                int userId = _userRepo.CreateUser(user);

                return CreatedAtAction(
                    nameof(CreateUser),
                    new
                    {
                        success = true,
                        message = "User created successfully",
                        userId = userId
                    });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Email already exists"))
                {
                    return Conflict(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating user",
                    error = ex.Message
                });
            }
        }
    }
}
