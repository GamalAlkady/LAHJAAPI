//namespace LAHJAAPI.Filters
//{
//    using Microsoft.OpenApi.Models;
//    using Swashbuckle.AspNetCore.SwaggerGen;

//    public class NullDefaultsSchemaFilter : ISchemaFilter
//    {
//        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//        {
//            if (schema?.Properties == null || context.Type.IsEnum) return;

//            foreach (var prop in schema.Properties)
//            {
//                var propertyInfo = context.Type.GetProperty(prop.Key);
//                if (propertyInfo != null)
//                {
//                    // إذا لم تكن الخاصية مطلوبة، اجعلها nullable وأزل القيمة الافتراضية
//                    if (!schema.Required.Contains(prop.Key))
//                    {
//                        prop.Value.Nullable = true;
//                        prop.Value.Default = null;
//                    }
//                }
//            }
//        }
//    }

//}
