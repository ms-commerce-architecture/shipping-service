using FluentValidation;
using shipping_service_backend.DTOs.Request;

namespace shipping_service_backend.Validators
{
    public class CreateShipmentRequestValidator : AbstractValidator<CreateShipmentRequest>
    {

        public CreateShipmentRequestValidator()
        {
            RuleFor(x => x.OrderNumber).NotEmpty().WithMessage("Order number is required.")
                .MaximumLength(100).WithMessage("Order number must not exceed 100 characters.");
            RuleFor(x => x.CustomerEmail).NotEmpty().WithMessage("custumer email is required.")
                .EmailAddress().WithMessage("Customer email must be a valid email address.");

            RuleFor(x => x.Carrier).MaximumLength(100).WithMessage("Carrier must not exceed 100 characters.")
                .When(x => x.Carrier is not null);

            RuleFor(x => x.EstimatedDelivery)
            .GreaterThan(DateTime.UtcNow).WithMessage("Estimated delivery must be a future date.")
            .When(x => x.EstimatedDelivery.HasValue);

        }
    }
}
