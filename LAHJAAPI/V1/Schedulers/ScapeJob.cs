using AutoGenerator.Schedulers;

namespace V1.Schedulers;



public class ScapeJob : BaseJob
{
    public override Task Execute(JobEventArgs context)
    {


        Console.WriteLine($"Executing job: {_options.JobName} with cron: {_options.Cron}");

        return Task.CompletedTask;
    }

    protected override void InitializeJobOptions()
    {

        // _options.
        _options.JobName = "space1";


    }
}