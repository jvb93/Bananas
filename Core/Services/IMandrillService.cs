using System.Collections.Generic;
using System.Threading.Tasks;
using Mandrill.Model;

namespace Core.Services
{
    public interface IMandrillService
    {
        Task<List<MandrillTemplateInfo>> GetTemplatesAsync();
        Task SendMessageAsync(string recipient, string subject, string fromAddress, string fromName, string templateName, Dictionary<string, string> templateFields);
    }
}