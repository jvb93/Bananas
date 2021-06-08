using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Classes;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Mandrill.Requests.Templates;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public class MandrillService : IMandrillService
    {
        private readonly IMandrillApi _mandrill;

        public MandrillService(IOptions<AppSettings> settings)
        {
            _mandrill = new MandrillApi(settings.Value.ApiKey);
        }

        public async Task<List<TemplateInfo>> GetTemplatesAsync()
        {
            var listTemplatesRequest = new ListTemplatesRequest();
            try
            {
                return await _mandrill.ListTemplates(listTemplatesRequest);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<TemplateInfo>();
            }
        }

        public async Task SendMessageAsync(string recipient, string subject, string fromAddress, string fromName, string templateName, Dictionary<string, string> templateFields)
        {
            var message = new EmailMessage()
            {
                RawTo = new List<string>() { recipient },
                FromEmail = fromAddress,
                FromName = fromName,
                Subject = subject
            };

            var templateContent = templateFields.Select(x => new TemplateContent()
            {
                Name = x.Key,
                Content = x.Value
            });
            
            var sendMessageTemplateRequest = new SendMessageTemplateRequest(message, templateName, templateContent);
            await _mandrill.SendMessageTemplate(sendMessageTemplateRequest);
        }
    }
}