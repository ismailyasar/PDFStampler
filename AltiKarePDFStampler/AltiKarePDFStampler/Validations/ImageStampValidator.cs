using System.Runtime.Intrinsics.X86;
using AltiKarePDFStampler.Models;
using FluentValidation;
using GemBox.Pdf.Content.Marked;

namespace AltiKarePDFStampler.Validations;

public class ImageStampValidator:AbstractValidator<ImageStamp>
{
    public ImageStampValidator()
    {
        RuleFor(x => x.FileBase64Text).NotNull().NotEmpty().WithMessage("FileBase64String cannot be null");
        RuleFor(x => x.ImageBase64Text).NotNull().NotEmpty().WithMessage("ImageBase64Text cannot be null");
        RuleFor(x => x.PageLocation).NotNull().WithMessage("Page location cannot be null").InclusiveBetween(1, 6).WithMessage("Location must be 1-6.");
        RuleFor(x => x.Height).GreaterThanOrEqualTo(16).WithMessage("Height value can be at least 16").LessThanOrEqualTo(200).WithMessage("Height value lees than 200");
        RuleFor(x => x.Width).GreaterThanOrEqualTo(16).WithMessage("Width value can be at least 16").LessThanOrEqualTo(200).WithMessage("Width value must be less than 200");
    }
}