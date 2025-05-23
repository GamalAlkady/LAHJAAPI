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
    public class SpaceService : BaseBPRServiceLayer<SpaceRequestDso, SpaceResponseDso, SpaceRequestShareDto, SpaceResponseShareDto>, IUseSpaceService
    {
        private readonly ISpaceShareRepository _share;
        public SpaceService(ISpaceShareRepository buildSpaceShareRepository, IMapper mapper, ILoggerFactory logger) : base(mapper, logger, buildSpaceShareRepository)
        {
            _share = buildSpaceShareRepository;
        }


        public async Task<IEnumerable<SpaceResponseDso>> GetSpacesBySubscriptionId(string subscriptionId)
        {
            try
            {
                _logger.LogInformation($"Retrieving spaces by subscription.");
                var pagedResponse = await _share.GetAllByAsync([new FilterCondition { PropertyName = "SubscriptionId", Value = subscriptionId }]);

                return MapToResponses(pagedResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieve spaces by subscription.");
                throw;
            }
        }

        public async Task<SpaceResponseDso> GetSpaceBySubscriptionId(string subscriptionId, string spaceId)
        {
            try
            {
                _logger.LogInformation($"Retrieving spaces by subscription.");
                var space = await _share.GetOneByAsync([
                    new FilterCondition { PropertyName = "Id", Value = spaceId },
                    new FilterCondition { PropertyName = "SubscriptionId", Value = subscriptionId }]);

                return MapToResponse(space);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieve spaces by subscription.");
                throw;
            }
        }
    }
}