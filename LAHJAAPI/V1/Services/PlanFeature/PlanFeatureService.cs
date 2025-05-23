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
    public class PlanFeatureService : BaseBPRServiceLayer<PlanFeatureRequestDso, PlanFeatureResponseDso, PlanFeatureRequestShareDto, PlanFeatureResponseShareDto>, IUsePlanFeatureService
    {
        private readonly IPlanFeatureShareRepository _share;
        public PlanFeatureService(IPlanFeatureShareRepository buildPlanFeatureShareRepository, IMapper mapper, ILoggerFactory logger) : base(mapper, logger, buildPlanFeatureShareRepository)
        {
            _share = buildPlanFeatureShareRepository;
        }

        public async Task<PlanFeatureResponseDso?> GetByNameAsync(string planId, string name, string lg = "en")
        {
            var response = await _share.GetAllByAsync([new FilterCondition("PlanId", planId)]);
            //_dbSet.Include(p => p.PlanFeatures).FirstOrDefaultAsync(p => p.Id == id);
            var planFeature = MapToResponses(response.Data);
            return planFeature.FirstOrDefault(p => p.Name?.Value?.Any(s => s.Value == name) == true);
        }

        public async Task<int> GetNumberRequests(string planId)
        {
            try
            {
                _logger.LogInformation("Getting number of requests for Plan ID: {PlanId}", planId);
                int numberRequests = 0;
                var planFeature = await GetByNameAsync(planId, "Requests", "en");
                var desc = planFeature.Description.Value["en"];
                if (desc.Contains("Unlimited", StringComparison.OrdinalIgnoreCase)) return int.MaxValue;
                if (desc.Contains("request")) desc = desc[..6];
                numberRequests = Convert.ToInt32(Convert.ToDecimal(desc));
                return numberRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting number of requests for Plan ID: {PlanId}", planId);
                return 0;
            }
        }

        public async Task<int> GetNumberSpaces(string planId)
        {
            try
            {
                _logger.LogInformation("Getting number of spaces for Plan ID: {PlanId}", planId);
                var planFeature = await GetByNameAsync(planId, "Space", "en");
                var feature = planFeature.Description.Value["en"];
                if (feature.Equals("Unlimited", StringComparison.OrdinalIgnoreCase)) return int.MaxValue;
                return Convert.ToInt32(Convert.ToDecimal(feature));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting number of spaces for Plan ID: {PlanId}", planId);
                return 0;
            }
        }


    }
}