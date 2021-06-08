using System.Collections.Generic;
using System.Threading.Tasks;
using Mandrill.Models;

namespace Core.Services
{
    public interface IMandrillService
    {
        Task<List<TemplateInfo>> GetTemplatesAsync();
        Task SendMessageAsync(string recipient, string subject, string fromAddress, string fromName, string templateName, Dictionary<string, string> templateFields);
    }
}