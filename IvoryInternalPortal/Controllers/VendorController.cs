using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using IvoryInternalPortal.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;

namespace IvoryInternalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendor _vendor;

        public VendorController(IVendor vendor)
        {
            _vendor = vendor;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var vendors = await _vendor.GetAllAsync();

                return Ok(new
                {
                    success = true,
                    count = vendors.Count,
                    data = vendors
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
        public async Task<IActionResult> InsertUpdate(
        [FromForm] Vendors vendor,
        IFormFile? agreementFile,
        IFormFile? signatureFile)
        {
            try
            {
                var result = await _vendor.InsertUpdateAsync(vendor, agreementFile, signatureFile);
            //return Ok(result);
                return Ok(new
                {
                    success = true,
                    message = vendor.VendorId == null || vendor.VendorId == 0
                        ? "Vendor created successfully"
                        : "Vendor updated successfully",
                    data = result
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
        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var vendor = await _vendor.GetByIdAsync(id);
            if (vendor == null)
                return NotFound(new { success = false, message = "Vendor not found" });

            return Ok(new { success = true, data = vendor });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _vendor.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { success = false, message = "Vendor not found" });

            return Ok(new { success = true, message = "Vendor deleted successfully" });
        }

        //    [HttpPost("register")]
        //    public IActionResult RegisterVendor(Vendors vendor, string password)
        //    {
        //        try
        //        {
        //            int userId;

        //            // check if user already exists
        //            int existingUserId = _userRepo.GetUserIdByEmail(vendor.Email);

        //            if (existingUserId > 0)
        //            {
        //                // email already exists
        //                userId = existingUserId;
        //            }
        //            else
        //            {
        //                // create new user
        //                Users user = new Users
        //                {
        //                    Email = vendor.Email,
        //                    Password = BCrypt.Net.BCrypt.HashPassword(password),
        //                    Role = 3 // Vendor
        //                };

        //                userId = _userRepo.CreateUser(user);
        //            }

        //            vendor.UserId = userId;

        //            // create vendor
        //            _vendorRepo.CreateVendor(vendor);

        //            return Ok("Vendor registered successfully");
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
    }
}
