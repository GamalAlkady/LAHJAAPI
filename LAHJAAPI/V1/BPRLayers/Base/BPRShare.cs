
using AutoGenerator;
using AutoMapper;

namespace V1.BPR.Layers.Base
{
    public interface IBaseBPRShareLayer<TRequest, TResponse> : IBPRLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
    }

    public abstract class BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse>
        : BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, ITBase, ITBase>, IBaseBPRShareLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
    {
        protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
        protected BaseBPRShareLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {
            _bpr = bpr;
        }

        public async Task<IEnumerable<TResponse>> GetAllAsync(string propertyName, object value, string[]? includes = null)
        {
            return Map<EResponse, TResponse>(await _bpr.GetAllAsync(propertyName, value, includes));
        }
    }
}