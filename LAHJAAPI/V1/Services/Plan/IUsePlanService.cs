using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUsePlanService : IPlanService<PlanRequestDso, PlanResponseDso>, IBaseService, IBaseBPRServiceLayer<PlanRequestDso, PlanResponseDso>
    {
        Task<PlanResponseDso> SetPlanAsync(PlanRequestDso entity);
    }
}