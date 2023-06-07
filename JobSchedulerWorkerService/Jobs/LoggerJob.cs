using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSchedulerWorkerService.Jobs
{
    public class LoggerJob : IJob
    {
            
        private readonly ILogger<LoggerJob> _logger;
        public LoggerJob(ILogger<LoggerJob> logger)
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
