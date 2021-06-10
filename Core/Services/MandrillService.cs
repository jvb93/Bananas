using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Classes;
using Mandrill;
using Mandrill.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public class MandrillService : IMandrillService
    {
        private readonly MandrillApi _mandrill;
        private readonly ILogger<MandrillService> _logger;
        
        public MandrillService(IOptions<MandrillSettings> settings, ILogger<MandrillService> logger)
        {
            _logger = logger;
            _mandrill = new MandrillApi(settings.Value.ApiKey);
        }

        public MandrillService(string apiKey, ILogger<MandrillService> logger)
        {
            _logger = logger;
            _mandrill = new MandrillApi(apiKey);
        }

        public async Task<List<MandrillTemplateInfo>> GetTemplatesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching Mandrill templates");
                return (await _mandrill.Templates.ListAsync()).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching templates");
                return new List<MandrillTemplateInfo>();
            }
        }

        public async Task SendMessageAsync(string recipient, string subject, string fromAddress, string fromName, string templateName, Dictionary<string, string> templateFields)
        {
            var message = new MandrillMessage();
            message.FromEmail = fromAddress;
            message.FromName = fromName;
            message.AddTo(recipient);
            message.ReplyTo = fromAddress;
            message.Subject = subject;
            foreach (var templateField in templateFields)
            {
                message.AddGlobalMergeVars(templateField.Key, templateField.Value);

            }
            var result = await _mandrill.Messages.SendTemplateAsync(message, templateName);
        }
    }
}