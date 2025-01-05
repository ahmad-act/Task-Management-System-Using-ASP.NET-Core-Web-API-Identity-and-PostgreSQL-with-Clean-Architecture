using TaskManagement.Application.DTOs.EntityPrototype;
using FluentValidation;

namespace TaskManagement.Application.Validation.EntityPrototype
{
    public class EntityPrototypeCreateDtoValidator : AbstractValidator<EntityPrototypeCreateDto>
    {
        public EntityPrototypeCreateDtoValidator()
        {
            //// Validate Id: Must not be null
            //RuleFor(updateDto => updateDto.Id)
            //    .NotNull()
            //    .WithMessage("Id must not be null.");

            // Validate Name: Required, minimum length of 3, and maximum length of 100
            RuleFor(updateDto => updateDto.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(3)
                .WithMessage("Name must not be less than 3 characters.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");

            // Validate Description: Optional but must meet length requirements if provided
            RuleFor(updateDto => updateDto.Description)
                .Cascade(CascadeMode.Stop)
                .Must(description => string.IsNullOrEmpty(description) || description.Length >= 3)
                .WithMessage("Description must not be less than 3 characters if provided.")
                .Must(description => string.IsNullOrEmpty(description) || description.Length <= 100)
                .WithMessage("Description must not exceed 100 characters if provided.");
        }

        public new Task<FluentValidation.Results.ValidationResult> ValidateAsync(EntityPrototypeCreateDto data, CancellationToken cancellationToken)
        {
            return base.ValidateAsync(data, cancellationToken);
        }
    }
}
