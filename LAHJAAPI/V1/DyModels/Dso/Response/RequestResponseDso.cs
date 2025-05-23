using AutoGenerator;
using V1.DyModels.Dto.Share.Responses;

namespace V1.DyModels.Dso.Responses
{
    public class RequestResponseDso : RequestResponseShareDto, ITDso
    {
        public string? EventId { get; internal set; }
        public string? AllowedRequests { get; internal set; }
        public int NumberRequests { get; internal set; }
    }
}