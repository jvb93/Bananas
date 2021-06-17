using System.Collections.Concurrent;
using System.Threading.Tasks;
using App.Core.Models;
using TaskStatus = App.Core.Models.TaskStatus;

namespace App.Core.Services
{
    public class MandrillTaskService : IMandrillTaskService
    {
        private readonly ConcurrentBag<TaskBatch> _taskBatches = new ConcurrentBag<TaskBatch>();

        public MandrillTaskService()
        {
        }

        public async Task EnqueueTaskBatch(TaskBatch taskBatch, IMandrillService mandrillService)
        {
            _taskBatches.Add(taskBatch);
            await Task.Run(async () =>
            {
                foreach (var mandrillTask in taskBatch.Tasks)
                {
                    if (!mandrillTask.ValidationResult.IsValid)
                    {
                        continue;
                    }
                    switch (mandrillTask)
                    {

                        case MandrillSendTemplateTask t:
                            var result = await mandrillService.SendMessageAsync(t.Recipient, t.Subject, t.FromAddress, t.FromName,
                                t.TemplateName, t.MergeVariables);
                            if (string.IsNullOrWhiteSpace(result))
                            {
                                t.Status = TaskStatus.Complete;
                            }
                            else
                            {
                                t.RejectReason = result;
                            }
                            break;

                        default: break;
                    }

                    await Task.Delay(250);
                }

                taskBatch.Finalize();
            });
        }
    }
}