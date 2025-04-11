using AutoGenerator.Schedulers;
using LAHJAAPI.V1.Validators.Conditions;
using Quartz;
using System.Reflection;

namespace LAHJAAPI.V1.Schedulers
{
    public static class ConfigMScheduler
    {


        public static void AddAutoConfigScheduler(this IServiceCollection serviceCollection)
        {
            Assembly? assembly = Assembly.GetExecutingAssembly();




            serviceCollection.AddHostedService(pro =>
           {
               var jober = pro.GetRequiredService<ISchedulerFactory>();

               var checker = new ConditionChecker(null);

               var jobs = ConfigScheduler.getJobOptions(checker, assembly);

               return new JobScheduler(jober, jobs);

           });








        }
    }
}