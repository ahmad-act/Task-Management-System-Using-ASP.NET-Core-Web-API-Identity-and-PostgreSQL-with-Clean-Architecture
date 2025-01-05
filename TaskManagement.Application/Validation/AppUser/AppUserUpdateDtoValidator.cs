using FluentValidation;
using TaskManagement.Application.DTOs.AuthDTOs.AppUser;

namespace TaskManagement.Application.Validation.AppUser
{
    public class AppUserUpdateDtoValidator : AbstractValidator<AppUserUpdateDto>
    {
        public AppUserUpdateDtoValidator()
        {
            // Validate Id: Must not be null
            RuleFor(updateDto => updateDto.UserName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("User name must not be less than 5 characters.")
                .MaximumLength(100)
                .WithMessage("User name must not exceed 100 characters.");

            // Validate Id: Must not be null
            RuleFor(updateDto => updateDto.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("Password must not be less than 5 characters.")
                .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters.");

            // Validate FieldName: Required, minimum length of 3, and maximum length of 100
            RuleFor(updateDto => updateDto.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MinimumLength(2)
                .WithMessage("First name must not be less than 2 characters.")
                .MaximumLength(30)
                .WithMessage("First name must not exceed 30 characters.");

            // Validate Description: Optional but must meet length requirements if provided
            RuleFor(updateDto => updateDto.LastName)
                .Cascade(CascadeMode.Stop)
                .Must(description => string.IsNullOrEmpty(description) || description.Length >= 2)
                .WithMessage("Last name must not be less than 2 characters if provided.")
                .Must(description => string.IsNullOrEmpty(description) || description.Length <= 30)
                .WithMessage("Last name must not exceed 30 characters if provided.");
        }

        public new Task<FluentValidation.Results.ValidationResult> ValidateAsync(AppUserUpdateDto data, CancellationToken cancellationToken)
        {
            return base.ValidateAsync(data, cancellationToken);
        }
    }
}
