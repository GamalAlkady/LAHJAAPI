using AutoGenerator.Helper;
using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class ServiceService : BaseBPRServiceLayer<ServiceRequestDso, ServiceResponseDso, ServiceRequestShareDto, ServiceResponseShareDto>, IUseServiceService
    {
        private readonly IServiceShareRepository _share;
        private readonly IUserServiceShareRepository _userServiceShare;

        public ServiceService(
            IServiceShareRepository buildServiceShareRepository,
            IUserServiceShareRepository userServiceShare,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger, buildServiceShareRepository)
        {
            _share = buildServiceShareRepository;
            _userServiceShare = userServiceShare;
        }

        public async Task<ServiceResponseDso> GetByAbsolutePath(string absolutePath)
        {
            var service = await _share.GetOneByAsync([new FilterCondition("AbsolutePath", absolutePath, FilterOperator.Contains)]);

            return MapToResponse(service);
        }

        private readonly HashSet<string> withoutServices = ["createspace"];
        public async Task<IEnumerable<ServiceResponseDso>> GetListWithoutSome(List<string>? servicesId = null, string? modelId = null)
        {
            List<FilterCondition> filterConditions = new List<FilterCondition>() {
                new FilterCondition("AbsolutePath", withoutServices, FilterOperator.NotIn)
            };
            if (servicesId != null) filterConditions.Add(new FilterCondition("Id", servicesId, FilterOperator.In));
            if (modelId != null) filterConditions.Add(new FilterCondition("ModelAiId", modelId));

            var response = await _share.GetAllByAsync(filterConditions);
            return MapToResponses(response.Data);
        }

        public async Task<IEnumerable<ServiceResponseDso>> GetUserServices(string userId)
        {
            try
            {
                _logger.LogInformation("Fetching UserServices...");
                var result = await _userServiceShare.GetAllByAsync(
                    [new FilterCondition("UserId", userId)]);
                if (result.TotalRecords == 0) return [];

                var services = result.Data.Select(s => s.Service);

                _logger.LogInformation("User fetched successfully.");
                return MapToResponses(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user.");
                throw;
            }
        }

        public new async Task<ServiceResponseDso> GetByName(string name)
        {
            var service = await _share.GetOneByAsync([new FilterCondition("Name", name)]);
            return MapToResponse(service);
        }


    }
}