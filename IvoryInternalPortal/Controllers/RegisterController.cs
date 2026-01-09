using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IvoryInternalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegister _register;

        public RegisterController(IRegister register)
        {
            _register = register;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _register.GetAllAsync();

            return Ok(new
            {
                success = true,
                count = users.Count,
                data = users
            });
        }

        [HttpPost("insert-update")]
        public async Task<IActionResult> InsertUpdate([FromBody] Register register)
        {
            var result = await _register.InsertUpdateAsync(register);

            return Ok(new
            {
                success = true,
                message = register.RegisterId == null || register.RegisterId == 0
                    ? "User registered successfully"
                    : "User updated successfully",
                data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _register.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new { success = true, data = user });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _register.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new
            {
                success = true,
                message = "User deactivated successfully"
            });
        }
    }
}