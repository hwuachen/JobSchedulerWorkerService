using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Quartz;
using Quartz.Spi;
using Quartz.Impl.AdoJobStore.Common;
using JobSchedulerWorkerService.Models;
using System.Collections.Specialized;
using JobSchedulerWorkerService.Jobs;
using Microsoft.Extensions.Logging;

namespace JobSchedulerWorkerService.Scheduler
{
    internal class SchedulerHostedService : IHostedService
    {      
        public IScheduler Scheduler { get; set; }
        private readonly IJobFactory jobFactory;
        private readonly List<JobInfo> jobInfo;
        private readonly ISchedulerFactory schedulerFactory;

        public SchedulerHostedService(
            ISchedulerFactory schedulerFactory, 
            List<JobInfo> jobInfo, 
            IJobFactory jobFactory)
        {         
            this.schedulerFactory = schedulerFactory;                       
            this.jobInfo = jobInfo;
            this.jobFactory = jobFactory;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var properties = new NameValueCollection();

            // and override values via builder
            Scheduler = await SchedulerBuilder.Create(properties)
                // default max concurrency is 10
                .UseInMemoryStore()
                .UseDefaultThreadPool(x => x.MaxConcurrency = 5)                
                .BuildScheduler();

            Scheduler.JobFactory = jobFactory;

            //Suporrt for Multiple Jobs
            jobInfo?.ForEach(jobInfo =>
            {
                //Create Job
                IJobDetail jobDetail = CreateJob(jobInfo);
                //Create trigger
                ITrigger trigger = CreateTrigger(jobInfo);
                //Schedule Job
                Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken).GetAwaiter();
                //Start The Schedular
            });
            await Scheduler.Start(cancellationToken);
        }

        private ITrigger CreateTrigger(JobInfo jobInfo)
        {
            return TriggerBuilder.Create()
                .WithIdentity(jobInfo.JobId.ToString())
                .WithCronSchedule(jobInfo.CronExpression)
                .WithDescription(jobInfo.JobName)
            .Build();
        }

        private IJobDetail CreateJob(JobInfo jobInfo)
        {
            return JobBuilder.Create(jobInfo.JobType)
                .WithIdentity(jobInfo.JobId.ToString())
                .WithDescription(jobInfo.JobName)
                .UsingJobData("JobName", jobInfo.JobName)
                .UsingJobData("CronExpression", jobInfo.CronExpression)
                .UsingJobData("Command", jobInfo.Command)
                .UsingJobData("Arguments", jobInfo.Arguments)                
                .Build();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           
            
             await Scheduler.Shutdown();
            
            
        }
    }

}
