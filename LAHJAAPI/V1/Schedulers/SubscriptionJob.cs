using AutoGenerator.Schedulers;
using LAHJAAPI.V1.Validators.Conditions;

namespace LAHJAAPI.V1.Schedulers
{


    public class SubscriptionJob : BaseJob
    {
        private readonly IConditionChecker _checker;
        public SubscriptionJob(IConditionChecker checker) : base()
        {

            _checker = checker;

        }
        public override Task Execute(JobEventArgs context)
        {
            Console.WriteLine($"Executing job: {_options.JobName} with cron: {_options.Cron}");
            return Task.CompletedTask;
        }

        protected override void InitializeJobOptions()
        {
            // _options.
            _options.JobName = "Subscription1";



        }
    }
}