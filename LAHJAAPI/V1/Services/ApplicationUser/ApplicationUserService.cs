using APILAHJA.Utilities;
using AutoGenerator;
using AutoGenerator.Helper;
using AutoGenerator.Services.Base;
using AutoMapper;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class ApplicationUserService : BaseService<ApplicationUserRequestDso, ApplicationUserResponseDso>, IUseApplicationUserService
    {
        private readonly IApplicationUserShareRepository _share;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IUseUserServiceService _userServiceService;
        private readonly IUseUserModelAiService _userModelAiService;
        public ApplicationUserService(
            IApplicationUserShareRepository buildApplicationUserShareRepository,
            IUserClaimsHelper userClaims,
            IMapper mapper,
            ILoggerFactory logger,
            IUseUserServiceService userServiceService,
            IUseUserModelAiService userModelAiService) : base(mapper, logger)
        {
            _share = buildApplicationUserShareRepository;
            _userClaims = userClaims;
            _userServiceService = userServiceService;
            _userModelAiService = userModelAiService;
        }

        public async Task<ApplicationUserResponseDso> GetUser()
        {
            try
            {
                _logger.LogInformation("Fetching user...");
                var result = await _share.GetByIdAsync(_userClaims.UserId)
                    ?? throw new ArgumentNullException($"User not found with Id: {_userClaims.UserId} that come from token.");
                _logger.LogInformation("User fetched successfully.");
                return GetMapper().Map<ApplicationUserResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user.");
                throw;
            }
        }

        public async Task<ApplicationUserResponseDso> GetUserWithSubscription()
        {
            try
            {
                _logger.LogInformation("Fetching user...");
                // TODO: test checkout
                var result = await _share.GetOneByAsync([new FilterCondition("Id", _userClaims.UserId)], new ParamOptions(["Subscription"]));
                _logger.LogInformation("User fetched successfully.");
                return GetMapper().Map<ApplicationUserResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user.");
                throw;
            }
        }

        public async Task<IEnumerable<ServiceResponseDso>?> GetServices(string? userId = null)
        {
            try
            {
                userId ??= _userClaims.UserId;
                _logger.LogInformation("Fetching user...");
                var result = await _share.GetOneByAsync(
                    [new FilterCondition("Id", userId)], new ParamOptions(["UserServices.Service"]));
                if (result == null) return null;

                var services = result.UserServices?.Select(s => new ServiceResponseDso
                {
                    Id = s.Service?.Id,
                    Name = s.Service?.Name,
                    AbsolutePath = s.Service?.AbsolutePath,
                    ModelAiId = s.Service?.ModelAiId,
                    Token = s.Service?.Token,
                });

                _logger.LogInformation("User fetched successfully.");
                return services;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user.");
                throw;
            }
        }

        public async Task<IEnumerable<ModelAiResponseDso>?> GetModels(string? userId = null)
        {
            try
            {
                userId ??= _userClaims.UserId;
                _logger.LogInformation("Fetching user...");
                var result = await _share.GetOneByAsync(
                    [new FilterCondition("Id", userId)], new ParamOptions(["UserModelAis.ModelAi"]));
                if (result == null) return null;
                var models = result.UserModelAis?.Select(s => new ModelAiResponseDso
                {
                    Id = s.ModelAi?.Id,
                    Name = s.ModelAi?.Name,
                    AbsolutePath = s.ModelAi?.AbsolutePath,
                    Token = s.ModelAi?.Token,
                });
                _logger.LogInformation("User fetched successfully.");
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user.");
                throw;
            }
        }

        public override Task<int> CountAsync()
        {
            try
            {
                _logger.LogInformation("Counting ApplicationUser entities...");
                return _share.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountAsync for ApplicationUser entities.");
                return Task.FromResult(0);
            }
        }

        public async Task<UserModelAiResponseDso?> AssignModelAi(string modelAiId, string? userId = null)
        {
            try
            {
                _logger.LogInformation("Assigning ModelAi to user...");
                userId ??= _userClaims.UserId;
                var userModel = await _userModelAiService.CreateAsync(new UserModelAiRequestDso
                {
                    ModelAiId = modelAiId,
                    UserId = userId
                });
                _logger.LogInformation("ModelAi assigned successfully.");
                return GetMapper().Map<UserModelAiResponseDso>(userModel);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ModelAi ID or User ID is null.");
                return null;
            }
            catch (Exception)
            {
                _logger.LogError("Error while assigning ModelAi to user.");
                throw;
            }
        }

        public async Task<UserServiceResponseDso?> AssignService(string serviceId, string? userId = null)
        {
            try
            {
                _logger.LogInformation("Assigning Service to user...");
                userId ??= _userClaims.UserId;
                var userService = await _userServiceService.CreateAsync(new UserServiceRequestDso
                {
                    ServiceId = serviceId,
                    UserId = userId
                });
                _logger.LogInformation("Service assigned successfully.");
                return GetMapper().Map<UserServiceResponseDso>(userService);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Service ID or User ID is null.");
                return null;
            }
            catch (Exception)
            {
                _logger.LogError("Error while assigning Service to user.");
                throw;
            }
        }
        public override async Task<ApplicationUserResponseDso> CreateAsync(ApplicationUserRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Creating new ApplicationUser entity...");
                var result = await _share.CreateAsync(entity);
                var output = GetMapper().Map<ApplicationUserResponseDso>(result);
                _logger.LogInformation("Created ApplicationUser entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating ApplicationUser entity.");
                return null;
            }
        }

        public override Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting ApplicationUser entity with ID: {id}...");
                return _share.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting ApplicationUser entity with ID: {id}.");
                return Task.CompletedTask;
            }
        }

        public override async Task<IEnumerable<ApplicationUserResponseDso>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all ApplicationUser entities...");
                var results = await _share.GetAllAsync();
                return GetMapper().Map<IEnumerable<ApplicationUserResponseDso>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for ApplicationUser entities.");
                return null;
            }
        }

        public override async Task<ApplicationUserResponseDso?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Retrieving ApplicationUser entity with ID: {id}...");
                var result = await _share.GetByIdAsync(id);
                var item = GetMapper().Map<ApplicationUserResponseDso>(result);
                _logger.LogInformation("Retrieved ApplicationUser entity successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync for ApplicationUser entity with ID: {id}.");
                return null;
            }
        }

        public override IQueryable<ApplicationUserResponseDso> GetQueryable()
        {
            try
            {
                _logger.LogInformation("Retrieving IQueryable<ApplicationUserResponseDso> for ApplicationUser entities...");
                var queryable = _share.GetQueryable();
                var result = GetMapper().ProjectTo<ApplicationUserResponseDso>(queryable);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetQueryable for ApplicationUser entities.");
                return null;
            }
        }

        public override async Task<ApplicationUserResponseDso> UpdateAsync(ApplicationUserRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Updating ApplicationUser entity...");
                var result = await _share.UpdateAsync(entity);
                return GetMapper().Map<ApplicationUserResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for ApplicationUser entity.");
                return null;
            }
        }

        public override async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                _logger.LogInformation("Checking if ApplicationUser exists with {Key}: {Value}", name, value);
                var exists = await _share.ExistsAsync(value, name);
                if (!exists)
                {
                    _logger.LogWarning("ApplicationUser not found with {Key}: {Value}", name, value);
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of ApplicationUser with {Key}: {Value}", name, value);
                return false;
            }
        }

        public override async Task<PagedResponse<ApplicationUserResponseDso>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all ApplicationUsers with pagination: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                var results = (await _share.GetAllAsync(includes, pageNumber, pageSize));
                var items = GetMapper().Map<List<ApplicationUserResponseDso>>(results.Data);
                return new PagedResponse<ApplicationUserResponseDso>(items, results.PageNumber, results.PageSize, results.TotalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all ApplicationUsers.");
                return new PagedResponse<ApplicationUserResponseDso>(new List<ApplicationUserResponseDso>(), pageNumber, pageSize, 0);
            }
        }

        public override async Task<ApplicationUserResponseDso?> GetByIdAsync(object id)
        {
            try
            {
                _logger.LogInformation("Fetching ApplicationUser by ID: {Id}", id);
                var result = await _share.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("ApplicationUser not found with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved ApplicationUser successfully.");
                return GetMapper().Map<ApplicationUserResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving ApplicationUser by ID: {Id}", id);
                return null;
            }
        }

        public override async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                _logger.LogInformation("Deleting ApplicationUser with {Key}: {Value}", key, value);
                await _share.DeleteAsync(value, key);
                _logger.LogInformation("ApplicationUser with {Key}: {Value} deleted successfully.", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ApplicationUser with {Key}: {Value}", key, value);
            }
        }

        public override async Task DeleteRange(List<ApplicationUserRequestDso> entities)
        {
            try
            {
                var builddtos = entities.OfType<ApplicationUserRequestShareDto>().ToList();
                _logger.LogInformation("Deleting {Count} ApplicationUsers...", 201);
                await _share.DeleteRange(builddtos);
                _logger.LogInformation("{Count} ApplicationUsers deleted successfully.", 202);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting multiple ApplicationUsers.");
            }
        }

        public override async Task<PagedResponse<ApplicationUserResponseDso>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving all ApplicationUser entities...");
                var results = await _share.GetAllAsync();
                var response = await _share.GetAllByAsync(conditions, options);
                return response.ToResponse(GetMapper().Map<IEnumerable<ApplicationUserResponseDso>>(response.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for ApplicationUser entities.");
                return null;
            }
        }

        public override async Task<ApplicationUserResponseDso?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving ApplicationUser entity...");
                return GetMapper().Map<ApplicationUserResponseDso>(await _share.GetOneByAsync(conditions, options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOneByAsync  for ApplicationUser entity.");
                return null;
            }
        }
    }
}