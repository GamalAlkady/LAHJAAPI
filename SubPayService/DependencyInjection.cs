using AutoGenerator.Conditions;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{

    private static IServiceCollection AddSubPayService(this IServiceCollection services, IBaseConditionChecker conditionChecker)
    {
        //services.AddScoped<IStripePrice, StripePrice>();



        //services.AddAutoMapper(typeof(StripeMappingConfig));
        return services;
    }
    public static IServiceCollection AddSubPayService(this IServiceCollection services,
        IBaseConditionChecker conditionChecker,
        IConfiguration configuration)
    {
        //StripeConfiguration.ApiKey = configuration["stripe:options:SecretKey"];
        //return services.AddServices();
        return services;
    }
}
