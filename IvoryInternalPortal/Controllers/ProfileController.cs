using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using IvoryInternalPortal.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IvoryInternalPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfile _repo;

        public ProfileController(IProfile repo)
        {
            _repo = repo;
        }

        // GET ALL
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var profiles = await _repo.GetAllAsync();

                return Ok(new
                {
                    success = true,
                    count = profiles.Count,
                    data = profiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error fetching profiles",
                    error = ex.Message
                });
            }
        }

        // GET BY ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var profile = await _repo.GetByIdAsync(id);

                if (profile == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Profile not found"
                    });

                return Ok(new
                {
                    success = true,
                    data = profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error fetching profile",
                    error = ex.Message
                });
            }
        }

        // INSERT / UPDATE
        [HttpPost("insert-update")]
        public async Task<IActionResult> InsertUpdate([FromBody] Profiles model)
        {
            try
            {
                var savedProfile = await _repo.InsertUpdateAsync(model);

                return Ok(new
                {
                    success = true,
                    message = model.ProfileId == null || model.ProfileId == 0
                        ? "Profile created successfully"
                        : "Profile updated successfully",
                    data = savedProfile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error saving profile",
                    error = ex.Message
                });
            }
        }

        // DELETE (SOFT DELETE)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repo.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new
                    {
                        success = false,
                        message = "Profile not found"
                    });

                return Ok(new
                {
                    success = true,
                    message = "Profile deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error deleting profile",
                    error = ex.Message
                });
            }
        }


        [HttpPost("preview-pdf")]
        public IActionResult PreviewPdf([FromBody] Profiles model)
        {
            try
            {
                var pdfBytes = ProfilePdfGenerator.Generate(model);
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("generate-pdf/{id:int}")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            var profile = await _repo.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            var pdfBytes = ProfilePdfGenerator.Generate(profile);
            return File(pdfBytes, "application/pdf", $"{profile.Name}.pdf");
        }


    }
}