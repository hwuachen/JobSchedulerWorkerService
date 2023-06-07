using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSchedulerWorkerService.Models
{
    public class JobInfo
    {
        public Guid JobId { get; set; }

        public Type JobType { get; set; }

        public string JobName { get; set; }

        public string CronExpression { get; set; }

        public string Command { get; set; }

        public string Arguments { get; set; }

        public JobInfo(Guid Id, Type jobType, string jobName,
                        string cronExpression, string command, string arguments)
        {
            JobId = Id;
            JobType = jobType;
            JobName = jobName;
            CronExpression = cronExpression;
            Command = command;
            Arguments = arguments;
        }
    }
}
