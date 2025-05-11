using AutoGenerator.Conditions;
using LAHJAAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LAHJAAPI.V1.Validators.Conditions
{
    public class SingletonContextFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SingletonContextFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public DataContext GetDataContext()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                // ÇÓÊÎÏã ÇáÓíÇÞ åäÇ ÈÃãÇä
                return context;
            }
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                return context.Set<TEntity>();
            }
        }

        // لتنفيذ دالة ترجع قيمة T
        public async Task<T> ExecuteInScopeAsync<T>(Func<DataContext, Task<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            return await action(context);
        }

        // لتنفيذ دالة غير مترجعة (void)
        public void ExecuteInScope(Action<DataContext> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            action(context);
        }
    }


    public static class ConfigValidator
    {
        public static IServiceCollection AddAutoValidator(this IServiceCollection serviceCollection)
        {
            Assembly? assembly = Assembly.GetExecutingAssembly();
            serviceCollection.AddSingleton<SingletonContextFactory>();
            serviceCollection.AddSingleton<ITFactoryInjector, TFactoryInjector>();
            serviceCollection.AddSingleton<IConditionChecker, ConditionChecker>(pro =>
            {
                var injctor = pro.GetRequiredService<ITFactoryInjector>();
                var checker = new ConditionChecker(injctor);
                BaseConfigValidator.Register(checker, assembly);
                return checker;
            });
            return serviceCollection;
        }
    }
}