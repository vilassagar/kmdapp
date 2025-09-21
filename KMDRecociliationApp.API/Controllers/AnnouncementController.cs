using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Domain.Results;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AnnoncementController : ControllerBase
    {
        private readonly ILogger<AnnoncementController> _logger;
        private readonly AnnouncementRepository _announcementRepository;
        private object retAnnouncement;

        public AnnoncementController(AnnouncementRepository announcementRepository, ILogger<AnnoncementController> logger)
        {
            _logger = logger;
            _announcementRepository = announcementRepository;
        }

        //GET: api/announcement/getannouncement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementDTO>>> GetAnnouncement([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            //if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
                }
                DataFilter<AnnouncementResult> filter = new DataFilter<AnnouncementResult>()
                {
                    PageNumber = searchDTO.Page,
                    Limit = searchDTO.pageSize,
                    Filter = null,
                    SortName = Sorting.SortName,
                    Search = searchDTO.Search,
                    SortDirection = Sorting.SortDirection
                };
                DataReturn<AnnouncementResult> dataReturn = new DataReturn<AnnouncementResult>();
                dataReturn = _announcementRepository.GetAnnouncement(filter);
                return await Task.FromResult(Ok(dataReturn));
            }
        }
        // GET: api/announcement/5  
        [HttpGet("{announcementid:int}")]
        public async Task<IActionResult> GetAnnouncement(int announcementid)
        {
            try
            {
                var announcement = await _announcementRepository.GetAnnouncementByIdAsync(announcementid);
                if (announcement == null)
                {
                    return NotFound(new { Message = $"Announcement with ID {announcementid} not found" });
                }

                return Ok(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAnnouncement by ID: {ex.Message}", ex);
                return Problem(detail: "An error occurred while retrieving the announcement. Please try again later.");
            }
        }



        // POST: api/announcement
        [HttpPost]
        public async Task<IActionResult> AddAnnouncement([FromQuery] AnnouncementDTO announcement)
        {
            // if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
               // _announcementRepository.CurrentUser = HttpContext.User;
                Announcement retAnnouncement = null;
                if (announcement.Id == null || announcement.Id == 0)
                {
                    var tempAnnouncement = await _announcementRepository.CheckAnnouncementName(announcement);
                    //_announcementRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);
                    // _rolePermissionRepo.UserEmail = _associationRepository.UserEmail;



                    if (tempAnnouncement == null)
                    {
                        retAnnouncement = await _announcementRepository.AddAnnouncementAsync(announcement.Name, announcement.AnnouncementText, announcement.AnnouncementLocation,announcement, 1/*_announcementRepository.UserId*/);
                        if (retAnnouncement != null && retAnnouncement.Id > 0)
                        {
                            return Ok(new { Message = "Announcement created successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "Announcement create Failed" });
                        }
                    }
                    else
                    {
                        string error = $"Announcement Name: {announcement.Name} already exists";
                        return Conflict(new { Message = error });

                    }

                }

                AnnouncementResult announcementResult = new AnnouncementResult().CopyPolicyData(retAnnouncement);
                return await Task.FromResult(Ok(announcementResult));

                // return await Task.FromResult(Ok());
            }
           
        }

        // PUT: api/anncouncement/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementDTO announcementDTO)
        {
            if (id != announcementDTO.Id)
            {
                return BadRequest(new { Message = "The ID in the URL does not match the ID in the request body." });
            }

            var announcement = await _announcementRepository.GetAnnouncementByIdAsync(id);
            if (announcement == null)
            {
                return NotFound(new { Message = $"Announcement with ID {id} not found." });
            }
            // Update properties
            announcement.Name = announcementDTO.Name;
            announcement.AnnouncementText = announcementDTO.AnnouncementText;
            announcement.AnnouncementLocation = announcementDTO.AnnouncementLocation; // Ensure this field is updated
            //announcement.IsActive = announcementDTO.IsActive; // Optional: Update other properties if necessary
            announcement.IsCurrent = announcementDTO.IsCurrent;
            announcement.UpdatedAt = DateTime.UtcNow; // Update the timestamp
            announcement.UpdatedBy = 1; // Example: Set to the current user's ID

            try
            {
                await _announcementRepository.UpdateAnnouncementAsync(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the announcement.");
                return StatusCode(500, new { Message = "An error occurred while updating the announcement. Please try again." });
            }

            return Ok(new { Message = "Announcement updated successfully." });
        }


        // DELETE: api/announcement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            bool deleted = await _announcementRepository.DeleteAnnouncementAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"Announcement with ID {id} not found." });
            }

            return Ok(new { Message = "Announcement Deleted successfully" });
        }

        // GET: api/announcement/announcementgetcurrent
        [HttpGet("GetCurrentAnnouncement")]
        public async Task<IActionResult> GetCurrentAnnouncement(string location)
        {
            try
            {
                var announcement = await _announcementRepository.GetCurrentAnnouncementAsync(location);
                if (announcement == null)
                {
                    return NotFound(new { Message = "No current announcement found" });
                }

                return Ok(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCurrentAnnouncement: {ex.Message}", ex);
                return Problem(detail: "An error occurred while retrieving the current announcement. Please try again later.");
            }
        }

    }
}