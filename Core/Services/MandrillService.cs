using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Classes;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Mandrill.Requests.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public class MandrillService : IMandrillService
    {
        private readonly IMandrillApi _mandrill;
        private readonly ILogger<MandrillService> _logger;
        
        public MandrillService(IOptions<AppSettings> settings, ILogger<MandrillService> logger)
        {
            _logger = logger;
            _mandrill = new MandrillApi(settings.Value.ApiKey);
        }

        public async Task<List<TemplateInfo>> GetTemplatesAsync()
        {
            var listTemplatesRequest = new ListTemplatesRequest();
            try
            {
                _logger.LogInformation("Fetching Mandrill templates");
                return await _mandrill.ListTemplates(listTemplatesRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching templates");
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