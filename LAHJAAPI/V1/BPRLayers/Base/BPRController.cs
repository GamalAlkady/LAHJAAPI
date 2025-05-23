
using AutoGenerator.Controllers.Base;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace V1.BPR.Layers.Base
{
    public abstract class CustomizeBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> where TRequest : class where TResponse : class where VMCreate : class where VMOutput : class where VMUpdate : class where VMInfo : class where VMDelete : class
    {
        private readonly IBPR<TRequest, TResponse> _bPR;

        protected CustomizeBPRController(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR)
        {
            _bPR = bPR;
        }


        // I used _bpr.CreateAsync(entity) insteadof  _bPR.CreateDataResultAsync(entity); to show errors when use throw
        // As example I used throw in RequestService when override CreateAsync(RequestRequestDso entity)

        public override async Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {
            if (!base.ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed on create.");
                return BadRequest("Invalid model");
            }

            TRequest entity = _mapper.Map<TRequest>(model);
            TResponse response = await _bPR.CreateAsync(entity);


            VMOutput value = _mapper.Map<VMOutput>(response);
            return Ok(value);
        }

        [NonAction]
        public override Task<ActionResult<VMOutput>> UpdateAsync([FromBody] VMUpdate model)
        {
            return base.UpdateAsync(model);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(400)]
        public virtual async Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed on update.");
                return BadRequest("Invalid model");
            }

            // جلب الكائن الأصلي القابل للتعديل من قاعدة البيانات
            var existingResult = await _bPR.GetByIdAsync(id);
            if (existingResult == null)
            {
                _logger.LogWarning("Item with ID {Id} not found for update.", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Update Failed",
                    Detail = "Item not found"
                });
            }

            // دمج القيم الجديدة مع الكائن الأصلي
            model.PatchProperties(existingResult);

            // تنفيذ التحديث
            var updateResult = await _bPR.UpdateDataResultAsync(_mapper.Map<TRequest>(existingResult));
            if (!updateResult.Success || updateResult.Data == null)
            {
                _logger.LogWarning("Failed to update item: {Message}", updateResult.Message);
                return NotFound(new ProblemDetails
                {
                    Title = "Update Failed",
                    Detail = updateResult.Message ?? "Item not found"
                });
            }

            // تحويل الكائن النهائي إلى ViewModel للعرض
            var output = _mapper.Map<TResponse>(updateResult.Data);
            return Ok(output);
        }


        public override async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation("Delete Item with {Id}.", id);
                await _bPR.DeleteAsync(id);
                _logger.LogInformation("Item with ID {Id} deleted successfully.", id);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to delete item with ID {Id}: {Message}", id, ex.Message);
                return NotFound(new ProblemDetails
                {
                    Title = "Delete Failed",
                    Detail = (ex.Message ?? ("Item with ID " + id + " not found"))
                });
            }
        }
    }

    // Without endpoint update 
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMInfo, VMDelete>
      : CustomizeBPRController<TRequest, TResponse, VMCreate, VMOutput, object, VMInfo, VMDelete> where TRequest : class where TResponse : class where VMCreate : class where VMOutput : class where VMInfo : class where VMDelete : class
    {
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR) { }


        [NonAction]
        public override Task<ActionResult<VMOutput>> UpdateAsync(string id, [FromBody] object model)
        {
            return base.UpdateAsync(model);
        }
    }

    // Without endpoints create and update
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMOutput, VMInfo, VMDelete>
    : BaseBPRControllerLayer<TRequest, TResponse, object, VMOutput, VMInfo, VMDelete> where TRequest : class where TResponse : class where VMOutput : class where VMInfo : class where VMDelete : class
    {
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR)
        {

        }

        [NonAction]
        public override Task<ActionResult<VMOutput>> CreateAsync([FromBody] object model)
        {
            return base.CreateAsync(model);
        }
    }

    // full endpoints
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
       : CustomizeBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
       where TRequest : class
       where TResponse : class
       where VMCreate : class
       where VMOutput : class
       where VMUpdate : class
       where VMInfo : class
       where VMDelete : class
    {
        protected IBaseBPRServiceLayer<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRServiceLayer<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR)
        {
            _bpr = bPR;
        }


    }


    /////////// ControllerLayerWithFilter LG/////////////
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFilter>
        : CustomizeBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
        where TRequest : class
        where TResponse : class
        where VMCreate : class
        where VMOutput : class
        where VMUpdate : class
        where VMInfo : class
        where VMDelete : class
        where VMFilter : class
    {
        protected IBaseBPRServiceLayer<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRServiceLayer<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR)
        {
            _bpr = bPR;
        }


        [HttpPost("GetByLanguage")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public virtual async Task<ActionResult<VMOutput>> GetByLanguage([FromBody] string? id, [FromQuery] string? lg)
        {
            string lg2 = lg;
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("Invalid ID received.");
                    return BadRequest("Invalid ID.");
                }

                _logger.LogInformation("Fetching item with ID: {id}", id);
                DataResult<TResponse> dataResult = await _bPR.GetByIdDataResultAsync(id);
                if (!dataResult.Success || dataResult.Data == null)
                {
                    _logger.LogWarning("Item not found with ID: {id}", id);
                    return NotFound(new ProblemDetails
                    {
                        Title = "Item Not Found",
                        Detail = (dataResult.Message ?? ("No item found with ID: " + id))
                    });
                }

                VMOutput value = _mapper.Map(dataResult.Data, delegate (IMappingOperationOptions<object, VMOutput> opt)
                {
                    opt.Items.Add(HelperTranslation.KEYLG, lg2);
                });
                return Ok(value);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while fetching item by ID.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetAllByLanguage")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public virtual async Task<ActionResult<IEnumerable<VMOutput>>> GetAllByLanguage([FromQuery] string? lg)
        {
            string lg2 = lg;
            if (string.IsNullOrWhiteSpace(lg2))
            {
                _logger.LogWarning("Language is null or empty.");
                return BadRequest("Language parameter is required.");
            }

            try
            {
                _logger.LogInformation("Fetching all items with language: {lg}", lg2);
                DataResult<IEnumerable<TResponse>> dataResult = await _bPR.GetAllDataResultAsync();
                if (!dataResult.Success || dataResult.Data == null || !dataResult.Data.Any())
                {
                    _logger.LogWarning("No items found.");
                    return NotFound(new ProblemDetails
                    {
                        Title = "No Items Found",
                        Detail = (dataResult.Message ?? "No data found.")
                    });
                }

                IEnumerable<VMOutput> value = _mapper.Map(dataResult.Data, delegate (IMappingOperationOptions<object, IEnumerable<VMOutput>> opt)
                {
                    opt.Items.Add(HelperTranslation.KEYLG, lg2);
                });
                return Ok(value);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while fetching items.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }

    public static class ObjectPatchExtensions
    {

        public static void PatchProperties<TSource, TDestination>(this TSource source, TDestination destination)
        {
            if (source == null || destination == null)
                return;

            var sourceType = typeof(TSource);
            var destType = typeof(TDestination);

            foreach (var prop in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var sourceValue = prop.GetValue(source);

                // نتجاهل القيم الفارغة
                if (sourceValue == null)
                    continue;

                // نبحث عن خاصية مطابقة بالاسم ونوع متوافق في الكائن الهدف
                var destProp = destType.GetProperty(prop.Name);
                if (destProp != null && destProp.CanWrite && destProp.PropertyType.IsAssignableFrom(prop.PropertyType))
                {
                    destProp.SetValue(destination, sourceValue);
                }
            }
        }
    }
}