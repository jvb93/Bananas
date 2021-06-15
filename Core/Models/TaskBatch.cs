using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace App.Core.Models
{
    public class TaskBatch
    {
        public List<MandrillTask> Tasks { get; set; }
        public TaskStatus BatchStatus;

        public TaskBatch()
        {
            Tasks = new List<MandrillTask>();
        }

        public void Finalize()
        {
            if (Tasks.Any(x => x.Status == TaskStatus.InProgress))
            {
                BatchStatus = TaskStatus.InProgress;
            }
            else if (Tasks.All(x => x.Status == TaskStatus.Complete))
            {
                BatchStatus = TaskStatus.Complete;
            }
            else if (Tasks.All(x => x.Status == TaskStatus.Failed))
            {
                BatchStatus = TaskStatus.Failed;
            }
            else
            {
                BatchStatus = TaskStatus.CompleteWithErrors;
            }
        }
    }

    public enum TaskStatus
    {
        InProgress,
        ValidationFailure,
        Complete,
        CompleteWithErrors,
        Failed,
    }
}
