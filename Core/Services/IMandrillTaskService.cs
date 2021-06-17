using App.Core.Models;
using System.Threading.Tasks;

namespace App.Core.Services
{
    public interface IMandrillTaskService
    {
        Task EnqueueTaskBatch(TaskBatch taskBatch, IMandrillService mandrillService);
    }
}
