using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSchedulerWorkerService.Jobs
{
    public class ProcessJob : IJob
    {
        private readonly ILogger<ProcessJob> _logger;
        public ProcessJob(ILogger<ProcessJob> logger)
        {
            this._logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var command = context.JobDetail.JobDataMap.GetString("Command");
            var arguments = context.JobDetail.JobDataMap.GetString("Arguments");

            var process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;

            _logger.LogInformation($"Log JobName: {context.JobDetail.JobDataMap.GetString("JobName")}: at {DateTime.Now} and Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            process.Start();

            // Wait for the process to exit
            process.WaitForExit();

            return Task.CompletedTask;
        }
    }
}
