using JobSchedulerWorkerService;
using JobSchedulerWorkerService.JobFactory;
using JobSchedulerWorkerService.Jobs;
using JobSchedulerWorkerService.Models;
using JobSchedulerWorkerService.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Spi;
using Microsoft.Extensions.Logging;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IJobFactory, JobFactoryService>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

        #region Adding JobType
        services.AddSingleton<NotificationJob>();
        services.AddSingleton<LoggerJob>();
        services.AddSingleton<ProcessJob>();
        #endregion

        #region Adding Jobs 

        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appSettings.json")
                            .Build();
        var quartzSection = configuration.GetSection("Quartz");


        List<JobInfo> jobInfos = new List<JobInfo>();

        //loop through the quartzSection and add the jobInfo to the list
        foreach (var section in quartzSection.GetChildren())
        {
            if (section["JobType"] == "NotificationJob")
                jobInfos.Add(new JobInfo(Guid.NewGuid(), typeof(NotificationJob), section["JobName"], section["CronExpression"], section["Command"], section["Arguments"]));
            else if (section["JobType"] == "LoggerJob")
                jobInfos.Add(new JobInfo(Guid.NewGuid(), typeof(LoggerJob), section["JobName"], section["CronExpression"], section["Command"], section["Arguments"]));
            else if (section["JobType"] == "ProcessJob")
                jobInfos.Add(new JobInfo(Guid.NewGuid(), typeof(ProcessJob), section["JobName"], section["CronExpression"], section["Command"], section["Arguments"]));

        }

        services.AddSingleton(jobInfos);

        #endregion

        services.AddHostedService<SchedulerHostedService>();
       
    })
    .Build();

//cancel the application when Ctrl+C is pressed
Console.CancelKeyPress += async (sender, e) =>
{
    await host.StopAsync();
    host.Dispose();
};


host.Run();
