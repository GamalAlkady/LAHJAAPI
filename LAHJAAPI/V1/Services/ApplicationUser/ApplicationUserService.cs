using APILAHJA.Utilities;
using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.Exceptions;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;
using WasmAI.ConditionChecker.Base;

namespace V1.Services.Services
{
    public class ApplicationUserService : BaseBPRServiceLayer<ApplicationUserRequestDso, ApplicationUserResponseDso, ApplicationUserRequestShareDto, ApplicationUserResponseShareDto>, IUseApplicationUserService
    {
        private readonly IApplicationUserShareRepository _share;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IUseUserServiceService _userServiceService;
        private readonly IUseUserModelAiService _userModelAiService;
        private readonly IConditionChecker _checker;

        public ApplicationUserService(
            IApplicationUserShareRepository buildApplicationUserShareRepository,
            IUserClaimsHelper userClaims,
            IMapper mapper,
            ILoggerFactory logger,
            IUseUserServiceService userServiceService,
            IUseUserModelAiService userModelAiService,
            IConditionChecker checker
            ) : base(mapper, logger, buildApplicationUserShareRepository)
        {
            _share = buildApplicationUserShareRepository;
            _userClaims = userClaims;
            _userServiceService = userServiceService;
            _userModelAiService = userModelAiService;
            _checker = checker;
        }

        public async Task<ApplicationUserResponseDso> GetUser()
        {
            try
            {
                _logger.LogInformation("Fetching user...");
                var result = await _share.GetByIdAsync(_userClaims.UserId)
                    ?? throw new ArgumentNullException($"User not found with Id: {_userClaims.UserId} that come from token.");
                _logger.LogInformation("User fetched successfully.");
                return MapToResponse(result);
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
                return MapToResponse(result);
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


        public async Task<UserModelAiResponseDso?> AssignModelAi(string modelAiId, string? userId = null)
        {
            try
            {
                _logger.LogInformation("Assigning ModelAi to user...");
                userId ??= _userClaims.UserId;
                if (await _checker.CheckAndResultAsync(ApplicationUserValidatorStates.IsModelAssigned, new DataFilter
                {
                    Id = userId,
                    Value = modelAiId
                }) is { Success: true } res)
                {
                    throw new ProblemDetailsException(res);
                }

                var userModel = await _userModelAiService.CreateAsync(new UserModelAiRequestDso
                {
                    ModelAiId = modelAiId,
                    UserId = userId
                });
                _logger.LogInformation("ModelAi assigned successfully.");
                return userModel;
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
                if (await _checker.CheckAndResultAsync(ApplicationUserValidatorStates.CanAssignService, new DataFilter
                {
                    Id = userId,
                    Value = serviceId
                }) is { Success: false } res)
                {
                    throw new ProblemDetailsException(res);
                }

                var userService = await _userServiceService.CreateAsync(new UserServiceRequestDso
                {
                    ServiceId = serviceId,
                    UserId = userId
                });
                _logger.LogInformation("Service assigned successfully.");
                return userService;
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

    }
}