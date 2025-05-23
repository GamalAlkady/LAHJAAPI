
using AutoGenerator;
using AutoGenerator.Repositories.Base;
using AutoMapper;

namespace V1.BPR.Layers.Base
{
    public interface IBPRLayer<TRequest, TResponse> : IBPR<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        Task<IEnumerable<TResponse>> GetAllAsync(string propertyName, object value, string[]? includes = null);
    }

    public abstract class BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, IT, IE>
        : TBPR<TRequest, TResponse, ERequest, EResponse, IT, IE>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
        where IT : ITBase
        where IE : ITBase
    {
        protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
        protected BaseBPRLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {
            _bpr = bpr;
        }

        //public override Task<PagedResponse<TResponse>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        //{

        //}
    }
}