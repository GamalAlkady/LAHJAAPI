
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace V1.BPR.Layers.Base
{
    public abstract class HalfBaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> where TRequest : class where TResponse : class where VMCreate : class where VMOutput : class where VMUpdate : class where VMInfo : class where VMDelete : class
    {
        protected HalfBaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR) { }

        [NonAction]
        public override Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {
            return base.CreateAsync(model);
        }

        [NonAction]
        public override Task<ActionResult<VMOutput>> UpdateAsync([FromBody] VMUpdate model)
        {
            return base.UpdateAsync(model);
        }
    }


    public abstract class FullBaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> where TRequest : class where TResponse : class where VMCreate : class where VMOutput : class where VMUpdate : class where VMInfo : class where VMDelete : class
    {
        protected FullBaseBPRController(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR) { }


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
        public async Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
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

    }


    public abstract class FullBaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFelter>
        : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFelter>
        where TRequest : class where TResponse : class where VMCreate : class where VMOutput : class where VMUpdate : class where VMInfo : class where VMDelete : class where VMFelter : class
    {
        protected FullBaseBPRController(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
            : base(mapper, logger, bPR) { }



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
        public async Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
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
    }

    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
       : FullBaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
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

        public override async Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {
            if (!base.ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed on create.");
                return BadRequest("Invalid model");
            }

            TRequest entity = _mapper.Map<TRequest>(model);
            TResponse response = await _bpr.CreateAsync(entity);


            VMOutput value = _mapper.Map<VMOutput>(response);
            return Ok(value);
        }
    }


    /////////// ControllerLayerWithFilter LG/////////////
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFilter>
        : FullBaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFilter>
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