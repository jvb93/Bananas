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
        
        public MandrillService(IOptions<MandrillSettings> settings)
        {
            _mandrill = new MandrillApi(settings.Value.ApiKey);
        }

        public MandrillService(string apiKey)
        { 
            _mandrill = new MandrillApi(apiKey);
        }

        public async Task<List<MandrillTemplateInfo>> GetTemplatesAsync()
        {
            try
            {
               var templateEnumerable = await _mandrill.Templates.ListAsync();
               return templateEnumerable.ToList();
            }
            catch (Exception e)
            {
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