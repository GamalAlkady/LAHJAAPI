using AutoMapper;

namespace V1.BPR.Layers.Base
{
    public interface IBaseBPRServiceLayer<TRequest, TResponse> : IBaseBPRShareLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        //Task<TResponse> UpdateAsync(string id, TRequest entity);
    }


    public abstract class BaseBPRServiceLayer<TRequest, TResponse, ERequest, EResponse>
        : BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse>, IBaseBPRServiceLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
    {
        protected new readonly IBaseBPRShareLayer<ERequest, EResponse> _bpr;
        protected BaseBPRServiceLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRShareLayer<ERequest, EResponse> bpr)
            : base(mapper, logger, bpr)
        {
            _bpr = bpr;
        }

        //public override Task<bool> ExistsAsync(object value, string name = "Id")
        //{
        //    return _bpr.ExistsAsync(value, name);
        //}
        //public virtual async Task<TResponse> UpdateAsync(string id, TRequest entity)
        //{
        //    try
        //    {
        //        var model = await GetByIdAsync(id);
        //        if (model == null)
        //        {
        //            throw new Exception("Record not found. make sure that id is correct.");
        //        }

        //        return await base.UpdateAsync(entity);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        protected TResponse MapToResponse(object request)
        {
            return _mapper.Map<TResponse>(request);
        }

        protected IEnumerable<TResponse> MapToResponses(object request)
        {
            return _mapper.Map<IEnumerable<TResponse>>(request);
        }
    }
}