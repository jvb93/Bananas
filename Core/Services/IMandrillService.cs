using System.Collections.Generic;
using System.Threading.Tasks;
using Mandrill.Model;

namespace App.Core.Services
{
    public interface IMandrillService
    {
        Task<List<MandrillTemplateInfo>> GetTemplatesAsync();
        Task<string> SendMessageAsync(string recipient, string subject, string fromAddress, string fromName, string templateName, Dictionary<string, string> templateFields);
    }
}