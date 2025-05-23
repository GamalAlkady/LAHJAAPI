using AutoGenerator;
using V1.DyModels.Dto.Share.Requests;

namespace V1.DyModels.Dso.Requests
{
    public class PlanRequestDso : PlanRequestShareDto, ITDso
    {
    }

    public class PlanRequest2Dso : ITDso
    {
        public required string Id { get; set; }

    }
}