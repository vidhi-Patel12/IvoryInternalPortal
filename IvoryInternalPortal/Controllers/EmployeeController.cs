using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using IvoryInternalPortal.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence.Repositories;
using IUser = IvoryInternalPortal.Interface.IUser;
using BCrypt.Net;

namespace IvoryInternalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _repo;

        public EmployeeController(IEmployee repo)
        {
            _repo = repo;
        }
     
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var employees = await _repo.GetAllAsync();

                return Ok(new
                {
                    success = true,
                    count = employees.Count,
                    data = employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error fetching employees",
                    error = ex.Message
                });
            }
        }

        [HttpPost("insert-update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> InsertUpdate([FromForm] Employees model,IFormFile? profileFile, IFormFile? aadhaarFile,
        IFormFile? panFile, IFormFile? agreementFile, IFormFile? signatureFile)
            {
            try
            {
                var savedEmployee = await _repo.InsertUpdateAsync(model,profileFile,aadhaarFile,
                    panFile,agreementFile,signatureFile);

                return Ok(new
                {
                    success = true,
                    message = model.EmployeeId == null || model.EmployeeId == 0
                        ? "Employee created successfully"
                        : "Employee updated successfully",
                    data = savedEmployee
                });
            }
            catch (Exception ex)
            {
                // You can log ex here
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null)
                return NotFound(new { success = false, message = "Employee not found" });

            return Ok(new { success = true, data = emp });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _repo.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { success = false, message = "Employee not found" });

            return Ok(new { success = true, message = "Employee deleted successfully" });
        }
    }
}