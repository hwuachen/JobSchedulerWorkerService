using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace JobSchedulerWorkerService.Jobs
{
    class NotificationJob : IJob
    {
        private readonly ILogger<NotificationJob> _logger;
        public NotificationJob(ILogger<NotificationJob> logger)
        {
            this._logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Log JobName: {context.JobDetail.JobDataMap.GetString("JobName")}: at {DateTime.Now} and Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            return Task.CompletedTask;
        }
    }
}
