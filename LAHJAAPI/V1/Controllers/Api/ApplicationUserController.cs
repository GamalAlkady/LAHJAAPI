using AutoGenerator.Helper.Translation;
using AutoMapper;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IUseApplicationUserService _applicationuserService;
        private readonly IConditionChecker _checker;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ApplicationUserController(
            IUseApplicationUserService applicationuserService,
            IConditionChecker checker,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _applicationuserService = applicationuserService;
            _checker = checker;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ApplicationUserController).FullName);
            //_logger.LogWarning("User:" + User);
        }

        // Get all ApplicationUsers.
        [HttpGet(Name = "GetApplicationUsers")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ApplicationUserOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all ApplicationUsers...");
                var result = await _applicationuserService.GetAllAsync();
                var items = _mapper.Map<List<ApplicationUserOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all ApplicationUsers");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a ApplicationUser by ID.
        [HttpGet("{id}", Name = "GetApplicationUser")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationUserOutputVM>> GetById(string id, string? lg)
        {

            try
            {
                _logger.LogInformation("Fetching ApplicationUser with ID: {id}", id);
                var entity = await _applicationuserService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("ApplicationUser not found with ID: {id}", id);
                    return NotFound();
                }
                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<ApplicationUserOutputVM>(entity));

                return Ok(_mapper.Map<ApplicationUserOutputVM>(entity, opts => opts.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ApplicationUser with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a ApplicationUsers by Lg.
        [HttpGet("GetApplicationUsersByLanguage", Name = "GetApplicationUsersByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ApplicationUserOutputVM>>> GetApplicationUsersByLg(string? lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid ApplicationUser lg received.");
                return BadRequest("Invalid ApplicationUser lg null ");
            }

            try
            {
                var applicationusers = await _applicationuserService.GetAllAsync();
                if (applicationusers == null)
                {
                    _logger.LogWarning("ApplicationUsers not found  by  ");
                    return NotFound();
                }

                var items = _mapper.Map<IEnumerable<ApplicationUserOutputVM>>(applicationusers, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ApplicationUsers with Lg: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost("AssignService", Name = "AssignService")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> AssignService(AssignServiceRequestVM assignService)
        {
            try
            {
                _logger.LogInformation("Assigning service to user with ID: {id}", assignService.ServiceId);
                if (await _checker.CheckAndResultAsync(ApplicationUserValidatorStates.CanAssignService, assignService.ServiceId) is { Success: true } res)
                {
                    return BadRequest(HandleResult.Text(res.Message ?? "Service already assigned."));
                }
                await _applicationuserService.AssignService(assignService.ServiceId);
                _logger.LogInformation("Service successfully assigned to user with ID: {id}", assignService.ServiceId);
                return Ok(HandleResult.Text("Service successfully assigned."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning service to user with ID: {id}", assignService.ServiceId);
                return BadRequest(HandleResult.Problem(ex));
            }
        }

        [HttpPost("AssignModelAi", Name = "AssignModelAi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> AssignModelAi(AssignModelRequestVM requestVM)
        {
            try
            {
                _logger.LogInformation("Assigning Model AI to user with ID: {id}", requestVM.ModelAiId);
                if (await _checker.CheckAndResultAsync(ApplicationUserValidatorStates.IsModelAssigned, requestVM.ModelAiId) is { Success: true } res)
                {
                    return BadRequest(HandleResult.Text(res.Message ?? "ModelAi already assigned."));
                }
                await _applicationuserService.AssignModelAi(requestVM.ModelAiId);
                _logger.LogInformation("Model AI successfully assigned to user with ID: {id}", requestVM.ModelAiId);
                return Ok(HandleResult.Text("Model AI successfully assigned."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning Model AI to user with ID: {id}", requestVM.ModelAiId);
                return BadRequest(HandleResult.Problem(ex));
            }
        }

        //[HttpPost("assignRole")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<UserResponse>> AssignRole(RoleAssign roleAssign)
        //{
        //    try
        //    {
        //        //var user = await userManager.FindByEmailAsync(roleAssign.Email);
        //        ApplicationUser user = (await userManager.FindByIdAsync(userClaims.UserId!))!;
        //        var roles = await userManager.GetRolesAsync(user);
        //        await userManager.RemoveFromRolesAsync(user, roles);

        //        var role = await roleManager.FindByIdAsync(roleAssign.RoleId);
        //        if (role != null)
        //        {
        //            IdentityResult result = await userManager.AddToRoleAsync(user, role.Name!);
        //            if (result.Succeeded)
        //            {
        //                var claims = await roleManager.GetClaimsAsync(role);
        //                //await userManager.AddClaimsAsync
        //                await userManager.UpdateAsync(user);
        //                await signInManager.RefreshSignInAsync(user);
        //                var response = mapper.Map<UserResponse>(user);
        //                return Ok(response);
        //            }
        //            else
        //                return Conflict(HandelErrors.Problem("Assign Role", "Connot assign this role to user"));
        //        }
        //        else
        //        {
        //            return NotFound(HandelErrors.NotFound($"There is no role with id {roleAssign.RoleId}"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(HandelErrors.Problem(ex));
        //    }
        //}

        // Delete a ApplicationUser.
        [HttpDelete("{id}", Name = "DeleteApplicationUser")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {

            try
            {
                _logger.LogInformation("Deleting ApplicationUser with ID: {id}", id);
                await _applicationuserService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ApplicationUser with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of ApplicationUsers.
        [HttpGet("CountApplicationUser")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting ApplicationUsers...");
                var count = await _applicationuserService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting ApplicationUsers");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}