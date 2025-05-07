using AutoGenerator.Schedulers;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators.Conditions;
using Quartz;
using System.Reflection;

namespace ApiCore.Schedulers
{

    public static class ConfigMScheduler
    {
        public static string GenerateCronExpression(int interval, ScheduleType type)
        {
            if (interval <= 0)
                throw new ArgumentException("Interval must be greater than 0");

            return type switch
            {
                ScheduleType.Minute when interval <= 59 => $"0 */{interval} * * * *",
                ScheduleType.Hour when interval <= 23 => $"0 0 */{interval} * * *",
                ScheduleType.Day when interval <= 31 => $"0 0 0 */{interval} * *",
                ScheduleType.Month when interval <= 12 => $"0 0 0 1 */{interval} *",
                ScheduleType.Year when interval >= 1 => $"0 0 0 1 1 */{interval}",
                _ => throw new ArgumentException("Invalid interval for the selected type")
            };
        }

        public static IServiceCollection AddAutoConfigScheduler(this IServiceCollection serviceCollection)
        {
            Assembly? assembly = Assembly.GetExecutingAssembly();
            serviceCollection.AddHostedService<JobScheduler>(pro =>
            {
                var jober = pro.GetRequiredService<ISchedulerFactory>();
                var checker = new ConditionChecker(null);
                var jobs = ConfigScheduler.getJobOptions(checker, assembly);
                return new JobScheduler(jober, jobs);
            });
            return serviceCollection;
        }
    }
}