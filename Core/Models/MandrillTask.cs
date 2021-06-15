using FluentValidation.Results;

namespace App.Core.Models
{
    public abstract class MandrillTask
    {
        private ValidationResult _validationResult;
        private string _rejectReason;
        public TaskStatus Status { get; set; }
        public ValidationResult ValidationResult => _validationResult ?? (_validationResult = Validate());

        public string RejectReason
        {
            get => _rejectReason;
            set
            {
                _rejectReason = value;
                if (!string.IsNullOrWhiteSpace(_rejectReason))
                {
                    Status = TaskStatus.Failed;
                }
            }
        }

        public abstract ValidationResult Validate();
    }
}