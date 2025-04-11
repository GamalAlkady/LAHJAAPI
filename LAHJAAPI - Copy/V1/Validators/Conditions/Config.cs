using AutoGenerator.Conditions;
using System.Reflection;

namespace V1.Validators.Conditions
{


    public static class ConfigValidator
    {
        public static void AddAutoValidator(this IServiceCollection serviceCollection)
        {


            Assembly? assembly = Assembly.GetExecutingAssembly();

            serviceCollection.AddScoped<ITFactoryInjector, TFactoryInjector>();
            serviceCollection.AddScoped<IConditionChecker, ConditionChecker>(pro =>
            {
                var injctor = pro.GetRequiredService<ITFactoryInjector>();

                var checker = new ConditionChecker(injctor);


                BaseConfigValidator.Register(checker, assembly);

                return checker;

            });




        }

    }


}

