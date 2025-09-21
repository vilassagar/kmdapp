using FluentValidation;
using KMDRecociliationApp.API.Common;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Results;
using Serilog;
using System.Net;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class AssociationController : ApiBaseController
    {
      
        private readonly ILogger<UserController> _logger;
        private readonly AssociationRepository _associationRepository;
        private readonly ApplicationDbContext _context;
       
        public AssociationController(AssociationRepository associationRepository,
           ApplicationDbContext context, ILogger<UserController> logger
           
            , RoleRepository roleRepository):base(context)
        {
           
            _logger = logger;
            _associationRepository = associationRepository;
            _context = context;
            
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAssociations([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
            
        {
          
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
                }
                DataFilter<AssociationResult> filter = new DataFilter<AssociationResult>()
                {
                    PageNumber = searchDTO.Page,
                    Limit = searchDTO.pageSize,
                    Filter = null,
                    SortName = Sorting.SortName,
                    Search = searchDTO.Search,
                    SortDirection = Sorting.SortDirection
                };
                DataReturn<AssociationResult> dataReturn = new DataReturn<AssociationResult>();
                dataReturn = _associationRepository.GetAssociations(filter);
                return await Task.FromResult(Ok(dataReturn));
            
           
        }

        [HttpPost("CreateAssociation")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAssociation([FromForm] DTOAssociation association)
        {
          
            {
                _associationRepository.CurrentUser = HttpContext.User;
                Association retAssociation = null;
                if (association.Id == null || association.Id == 0)
                {
                    var tempAssociation = await _associationRepository.CheckAssociationName(association);
                    _associationRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);
                  

                    if (tempAssociation == null
                        )
                    {
                        var mandateFileobj = association.MandateFile==null?null: association.MandateFile.File;
                        var qrCodeFileObj= association.QrCodeFile == null ? null : association.QrCodeFile.File;
                        retAssociation = await _associationRepository.CreateAssociationAsync(mandateFileobj, qrCodeFileObj, association, _associationRepository.UserId);
                        if (retAssociation != null && retAssociation.Id > 0)
                        {
                            return Ok(new { Message = "Association created successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "Association create Failed" });
                        }
                     
                    }
                    else
                    {
                        string error = $"Association Name: {association.AssociationName} already exists";
                        return Conflict(new { Message = error });
                      
                    }
                }

                AssociationResult associationResult = new AssociationResult().CopyPolicyData(retAssociation);
                return await Task.FromResult(Ok(associationResult));

               
            }
            
        }

        [HttpPatch("UpdateAssociation/{associationid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAssociation([FromForm] DTOAssociation association, int associationId)
        {
            
                _associationRepository.CurrentUser = HttpContext.User;
                Association retAssociation = null;
                if (association.Id != null || association.Id != 0)
                {
                    var tempAssociation = await _associationRepository.CheckAssociationName(association,true);
                    _associationRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);
                    


                    if (tempAssociation == null)
                    {
                        retAssociation = await _associationRepository.UpdateAssociationAsync(associationId, association.MandateFile.File, association.QrCodeFile.File, association, _associationRepository.UserId);
                        if (retAssociation != null && retAssociation.Id > 0)
                        {
                            return Ok(new { Message = "Association updated successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "Association update Failed" });
                        }
                        

                    }
                    else
                    {
                        string error = $"Association Name: {association.AssociationName} already exists";
                        return Conflict(new { Message = error });
                    }
                }
                 return await Task.FromResult(Ok());

               
         }
            

        [HttpGet("{associationid:int}")]
        public async Task<IActionResult> GetAssociationById(int associationid)
        {
           // if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                try
                {
                    var association = _associationRepository.GetAssociationById(associationid).Result;
                    return await Task.FromResult(Ok(association));
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Error in Getbyid {ex.Message}");
                    return await Task.FromResult(Problem(detail: $"Something went wrong!"));
                }
            }
         
        }

       
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteAssociation(int id)
        {
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                List<string> messages = new List<string>();
                try
                {

                    var association = await _associationRepository.GetAssociationById(id);
                    if (association == null)
                    {
                        messages.Add($"ApplicationRole Not Found ");
                        return await Task.FromResult(NotFound(messages));
                    }
                    return await Task.FromResult(Ok());
                    //var obj = _associationRepository.Delete(association, _associationRepository.UserEmail, _associationRepository.UserFullName, "ApplicationRole Deleted");

                    //if (obj != null)
                    //{
                    //    return await Task.FromResult(Ok(association));
                    //}
                    //else
                    //{
                    //    return await Task.FromResult(Problem(detail: $"Unable to delete!"));
                    //}
                }
                catch (Exception ex)
                {

                    return await Task.FromResult(Problem(detail: $"Something went wrong!"));
                }

            }
            else
            {
                return Unauthorized();
            }


        }


    }
}
