using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OrganisationController : ControllerBase
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly OrganisationRepository _organisationRepository;
        private object retOrganisation;

        public OrganisationController(OrganisationRepository organisationRepository, ILogger<OrganisationController> logger)
        {
            _logger = logger;
            _organisationRepository = organisationRepository;
        }

        //GET: api/organisation/getorganisations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganisationDTO>>> GetOrganisations([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            //if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
                }
                DataFilter<OrganisationResult> filter = new DataFilter<OrganisationResult>()
                {
                    PageNumber = searchDTO.Page,
                    Limit = searchDTO.pageSize,
                    Filter = null,
                    SortName = Sorting.SortName,
                    Search = searchDTO.Search,
                    SortDirection = Sorting.SortDirection
                };
                DataReturn<OrganisationResult> dataReturn = new DataReturn<OrganisationResult>();
                dataReturn = _organisationRepository.GetOrganisation(filter);
                return await Task.FromResult(Ok(dataReturn));
            }
        }

        // GET: api/Organisation/5
        [HttpGet("{organisationid:int}")]
        public async Task<IActionResult> GetOrganisation(int organisationid)
        {
            try
            {
                var organisation = await _organisationRepository.GetOrganisationByIdAsync(organisationid);
                if (organisation == null)
                {
                    return NotFound(new { Message = $"Organisation with ID {organisationid} not found" });
                }

                return Ok(organisation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOrganisation by ID: {ex.Message}", ex);
                return Problem(detail: "An error occurred while retrieving the organisation. Please try again later.");
            }
        }



        // POST: api/Organisation
        [HttpPost]
        public async Task<ActionResult<OrganisationDTO>> AddOrganisation([FromBody] OrganisationDTO organisation)
        {
            // if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                _organisationRepository.CurrentUser = HttpContext.User;
                Organisation retOrganisation = null;
                if (organisation.Id == null || organisation.Id == 0)
                {
                    var tempOrganisation = await _organisationRepository.CheckOrganisationName(organisation);
                    //_associationRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);
                    // _rolePermissionRepo.UserEmail = _associationRepository.UserEmail;



                    if (tempOrganisation == null)
                    {
                        retOrganisation = await _organisationRepository.AddOrganisationAsync(organisation);
                        if (retOrganisation != null && retOrganisation.Id > 0)
                        {
                            return Ok(new { Message = "Organisation created successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "Organisation create Failed" });
                        }
                        ////if (retAssociation != null && retAssociation.Id > 0)
                        ////{
                        ////    //Add premimum chart data
                        ////}

                    }
                    else
                    {
                        string error = $"Organisation Name: {organisation.Name} already exists";
                        return Conflict(new { Message = error });

                    }
                }

                OrganisationResult organisationResult = new OrganisationResult().CopyPolicyData(retOrganisation);
                return await Task.FromResult(Ok(organisationResult));

                // return await Task.FromResult(Ok());
            }
            //else
            //{
            //    return Unauthorized();
            //}
            /*var Organisation = new Organisation
            {
                Name = organisation.Name,
                Description = organisation.Description
            };
            var tempOrganisation = await _organisationRepository.CheckOrganisationNameAsync(organisation.Name);
            _organisationRepository.AddOrganisationAsync(organisation);
            if (tempOrganisation == null)
            {
                retOrganisation = await _organisationRepository.AddOrganisationAsync(Organisation.Name, organisation,_organisationRepository.UserFullName);
                if (retOrganisation != null && retOrganisation.Id > 0)
                {
                    return Ok(new { Message = "Organisation created successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "Organisation create Failed" });
                }
            }
            else
            {
                string error = $"Organisation Name: {organisation.Name} already exists";
                return Conflict(new { Message = error });

            }

            OrganisationResult organisationResult = new OrganisationResult().Copy(retOrganisation);
            return await Task.FromResult(Ok(OrganisationResult));*/
        }

        // PUT: api/Organisation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganisation(int id, [FromBody] OrganisationDTO organisationDto)
        {
            if (id != organisationDto.Id)
            {
                return BadRequest();
            }

            var organisation = await _organisationRepository.GetOrganisationByIdAsync(id);
            if (organisation == null)
            {
                return NotFound(new { Message = $"Organisation with ID {id} not found." });
            }

            organisation.Name = organisationDto.Name;
            organisation.Description = organisationDto.Description;

            try
            {
                await _organisationRepository.UpdateOrganisationAsync(organisation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the organisation.");
                return StatusCode(500, new { Message = "An error occurred while updating the organisation. Please try again." });
            }

            return Ok(new { Message = "Organisation updated successfully." });
        }

        // DELETE: api/Organisation/5
        [HttpDelete("{id}")]
public async Task<IActionResult> DeleteOrganisation(int id)
{
    bool deleted = await _organisationRepository.DeleteOrganisationAsync(id);
    if (!deleted)
    {
        return NotFound(new { Message = $"Organisation with ID {id} not found." });
    }

            return Ok(new { Message = "Organisation Deleted successfully" });
        }
    }
}
