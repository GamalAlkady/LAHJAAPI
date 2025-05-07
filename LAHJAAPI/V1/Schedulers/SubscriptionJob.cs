using AutoGenerator.Conditions;
using AutoGenerator.Schedulers;
using AutoNotificationService.Services.Email;
using LAHJAAPI.V1.Helper;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.VMs;

namespace ApiCore.Schedulers
{
    public class SubscriptionJob : BaseJob
    {
        private readonly IConditionChecker _checker;
        public SubscriptionJob(IConditionChecker checker) : base()
        {
            _checker = checker;


        }
        static bool isons = false;
        static DateOnly? date;
        static string confirmationLink = "https://example.com/confirm?token=123456";
        string renewalLink = "https://example.com/confirm?token=123456";



        //private async Task<List<string>> GetEmailByRequestCodtion() { }
        //private async Task<List<string>>GetEmailBySpacCodtion() { }

        public override async Task Execute(JobEventArgs context)
        {
            //if (date is null || date != DateOnly.FromDateTime(DateTime.Now))
            {

                //send notification if subscription is about to expire after 7 days or 2 days
                if (await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsExpireAfter, new DataFilter
                {
                    Value = new List<int> { 7, 2 }
                }) is { Success: true } result)
                {
                    var users = (List<ApplicationUserFilterVM>)result.Result;
                    foreach (var user in users)
                    {
                        //send email to each user
                        await _checker.Injector.Notifier.NotifyAsyn(new EmailModel()
                        {
                            Body = TemplateTagEmail.SubscriptionEndingSoonTemplate(user.Days, renewalLink),
                            Subject = "Renew your subscription",
                            ToEmail = user.Email
                        });
                    }
                    date = DateOnly.FromDateTime(DateTime.Now);
                }
            }


        }

        protected override void InitializeJobOptions()
        {
            // _options.
            _options.JobName = "IsSubscriptionExpireAfter";
            _options.Cron = CronSchedule.EveryHour;
        }
    }
}