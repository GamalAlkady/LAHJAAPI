using AutoMapper;
using LAHJAAPI.Services2;
using LAHJAAPI.V1.Validators.Conditions;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class ModelGatewayService : BaseBPRServiceLayer<ModelGatewayRequestDso, ModelGatewayResponseDso, ModelGatewayRequestShareDto, ModelGatewayResponseShareDto>, IUseModelGatewayService
    {
        private readonly IConditionChecker _checker;
        private readonly IModelGatewayShareRepository _share;
        public ModelGatewayService(
            IMapper mapper,
            ILoggerFactory logger,
            IConditionChecker checker,
            IModelGatewayShareRepository bpr) : base(mapper, logger, bpr)
        {
            _checker = checker;
            _share = bpr;
        }

        public override Task<ModelGatewayResponseDso> CreateAsync(ModelGatewayRequestDso entity)
        {
            entity.Token = TokenService.GenerateSecureToken();
            return base.CreateAsync(entity);
        }
        //public override async Task<ModelGatewayResponseDso> UpdateAsync(string id, ModelGatewayRequestDso entity)
        //{
        //    var modelGateway = await GetByIdAsync(id);
        //    if (modelGateway == null)
        //    {
        //        throw new Exception("Record not found. make sure that id is correct.");
        //    }
        //    entity.Id = id;
        //    //entity.Token = modelGateway.Token;

        //    return await base.UpdateAsync(entity);
        //}
        //public override async Task<bool> ExistsAsync(object value, string name = "Id")
        //{
        //    return await base.ExistsAsync(value, name);
        //}
    }
}