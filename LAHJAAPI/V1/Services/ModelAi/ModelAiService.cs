using AutoGenerator;
using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.Models;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.DyModels.VMs;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class ModelAiService : BaseBPRServiceLayer<ModelAiRequestDso, ModelAiResponseDso, ModelAiRequestShareDto, ModelAiResponseShareDto>, IUseModelAiService
    {
        private readonly IModelAiShareRepository _share;
        public ModelAiService(IMapper mapper, ILoggerFactory logger, IModelAiShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
        public async Task<PagedResponse<ModelAiResponseDso>> FilterMaodelAi(ModelAiFilterVM searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            List<FilterCondition> conditions = new List<FilterCondition>();
            //var query = await _repository.GetAll();


            if (!string.IsNullOrWhiteSpace(searchModel.Type))
            {
                conditions.Add(new FilterCondition
                {
                    PropertyName = nameof(ModelAi.Type),
                    Operator = FilterOperator.Equals,
                    Value = searchModel.Type
                });
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Name))
            {
                conditions.Add(new FilterCondition
                {
                    PropertyName = nameof(ModelAi.Name),
                    Operator = FilterOperator.Contains,
                    Value = searchModel.Name
                });
            }

            if ((!string.IsNullOrWhiteSpace(searchModel.Dialect)))
            {
                conditions.Add(new FilterCondition
                {
                    PropertyName = nameof(ModelAi.Dialect),
                    Operator = FilterOperator.Contains,
                    Value = searchModel.Dialect
                });
            }

            if ((!string.IsNullOrWhiteSpace(searchModel.Gender)))
            {
                conditions.Add(new FilterCondition
                {
                    PropertyName = nameof(ModelAi.Gender),
                    Operator = FilterOperator.Equals,
                    Value = searchModel.Gender
                });
            }

            if ((!string.IsNullOrWhiteSpace(searchModel.Language)))
            {
                conditions.Add(new FilterCondition
                {
                    PropertyName = nameof(ModelAi.Language),
                    Operator = FilterOperator.Equals,
                    Value = searchModel.Language
                });
            }
            if ((!string.IsNullOrWhiteSpace(searchModel.Category)))
            {
                conditions.Add(new FilterCondition { PropertyName = nameof(ModelAi.Category), Operator = FilterOperator.Contains, Value = searchModel.Category });
            }
            //TODO: error when implementing this
            if (searchModel.IsStandard != null)
            {
                //conditions.Add(new FilterCondition { PropertyName = nameof(ModelAi.IsStandard), Operator = FilterOperator.Contains, Value = (bool)searchModel.IsStandard });
            }

            if (conditions.Count == 0)
            {
                _logger.LogWarning("No filters provided for ModelAi search.");
                throw new ArgumentException("No filters provided for ModelAi search.");
            }
            var response = await _share.GetAllByAsync(conditions);
            //if (response.TotalRecords == 0)
            //{
            //    _logger.LogWarning("No ModelAi found with the provided filters.");
            //    return response.Data= Enumerable.Empty<ModelAiResponseDso>();
            //}


            return response.ToResponse(MapToResponses(response.Data));
        }
    }
}
