using FluentValidation;

namespace Bit.Lib.Domain.Validators
{
    public class FlagNameValidator : AbstractValidator<string>
    {
        public FlagNameValidator()
        {
            RuleFor(flagName => flagName)
                .NotEmpty().WithMessage("Flag name cannot be empty. Please provide a flag name.")
                .Length(2, 50).WithMessage("Flag name must be between 2 and 50 characters. Please adjust the length of your flag name.")
                .Matches(@"^[a-zA-Z0-9\:.\-_]+$").WithMessage("Flag name contains invalid characters. Only alphanumeric characters and the special characters ':', '.', '-', and '_' are allowed.")
                .Custom((flagName, context) => {
                    if (!IsValidFlagName(flagName))
                    {
                        context.AddFailure($"The flag name '{flagName}' is invalid. Please ensure it meets all the validation rules.");
                    }
                });
        }

        private bool IsValidFlagName(string flagName)
        {
            return flagName.Length > 0;
        }
    }
}