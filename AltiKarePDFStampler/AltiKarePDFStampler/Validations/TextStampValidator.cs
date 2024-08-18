using AltiKarePDFStampler.Models;
using FluentValidation;

namespace AltiKarePDFStampler.Validations;

public class TextStampValidator:AbstractValidator<TextStamp>
{
    public TextStampValidator()
    {
        RuleFor(x => x.FileBase64String).NotNull().NotEmpty().WithMessage("FileBase64String cannot be null");
        RuleFor(x => x.TextSize).NotEmpty().WithMessage("Text size value cannot be null").LessThanOrEqualTo(100).WithMessage("TextSize cannot be greater than 100 ");
        RuleFor(x => x.Text).NotEmpty().WithMessage("Text cannot be null").MaximumLength(200).WithMessage("Text size length cannot be greater 200");
        RuleFor(x => x.PageLocation).NotNull().NotEmpty().WithMessage("Page location cannot be null. Please type 1-6 value").InclusiveBetween(1, 6).WithMessage("Page location must be in 1-6");
    }
}