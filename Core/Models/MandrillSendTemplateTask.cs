using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace App.Core.Models
{
    public class MandrillSendTemplateTask : MandrillTask
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string> MergeVariables { get; set; }
        public override ValidationResult Validate()
        {
           var validator = new MandrillSendTemplateTaskValidator();
           var validationResult = validator.Validate(this);
           if (!validationResult.IsValid)
           {
               Status = TaskStatus.ValidationFailure;
           }

           return validationResult;
        }

        public class MandrillSendTemplateTaskValidator : AbstractValidator<MandrillSendTemplateTask>
        {
            public MandrillSendTemplateTaskValidator()
            {
                RuleFor(x => x.Subject).NotEmpty();
                RuleFor(x => x.Recipient).EmailAddress();
                RuleFor(x => x.FromAddress).EmailAddress();
                RuleFor(x => x.FromName).NotEmpty();
                RuleFor(x => x.TemplateName).NotEmpty();
            }
        }
    }
}
