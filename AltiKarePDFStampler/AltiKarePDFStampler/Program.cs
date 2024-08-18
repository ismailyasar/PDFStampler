using System.Net;
using System.Text;
using AltiKarePDFStampler.Models;
using AltiKarePDFStampler.Utilities;
using AltiKarePDFStampler.Validations;
using FluentValidation;
using FluentValidation.Results;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509.Qualified;

var builder = WebApplication.CreateBuilder(args);

//service register
builder.Services.AddValidatorsFromAssemblyContaining(typeof(TextStampValidator));

var app = builder.Build();

var licenseText = builder.Configuration.GetValue<string>("License:Key");

var license =
    Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(licenseText))));

app.MapGet("/hello", () => "I am ready!");

//Add text
app.MapPost("/addtext", ([FromBody] TextStamp textStamp, [FromServices] IValidator<TextStamp> validator) =>
{
    ValidationResult validationResults = validator.Validate(textStamp);

    if (validationResults.IsValid)
    {
        ComponentInfo.SetLicense(license);

        try
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(textStamp.FileBase64String));
            MemoryStream outputStream = new MemoryStream();

            using (var document = PdfDocument.Load(ms))
            {
                using (var formattedText = new PdfFormattedText())
                {
                    formattedText.Append(textStamp.Text);
                    formattedText.FontSize = textStamp.TextSize;
                    
                    foreach (var page in document.Pages)
                    {
                        page.Content.DrawText(formattedText,
                            Stamper.GetCoordinate(page, formattedText.Height, formattedText.Width,(Coordinate) textStamp.PageLocation));
                    }
                }

                document.Save(outputStream);
                ms.Dispose();
            }

            var outputBase64String = Convert.ToBase64String(outputStream.ToArray());
            var resultObj = new ResultModel()
            {
                ErrorMessages = Array.Empty<string>(), Data = outputBase64String, StatusCode = (int) HttpStatusCode.OK,
                Success = true
            };

            outputStream.Dispose();
            return Results.Ok(resultObj);
        }
        catch (Exception exception)
        {
            var errorMessages = new string[1] {exception.Message};
            return Results.BadRequest(new ResultModel()
            {
                Data = null, ErrorMessages = errorMessages, StatusCode = (int) HttpStatusCode.BadRequest,
                Success = false
            });
        }
    }

    var resultMessages = Stamper.GetValidationMessagesAsArray(validationResults);

    return Results.BadRequest(new ResultModel()
    {
        Data = null, ErrorMessages = resultMessages, StatusCode = (int) HttpStatusCode.UnprocessableEntity,
        Success = false
    });
});

//Add image
app.MapPost("/addimage", ([FromBody] ImageStamp imageStamp, [FromServices] IValidator<ImageStamp> validator) =>
{
    ValidationResult validationResults = validator.Validate(imageStamp);

    if (validationResults.IsValid)
    {
        ComponentInfo.SetLicense(license);
        
        try
        {
            MemoryStream sourceStream = new MemoryStream(Convert.FromBase64String(imageStamp.FileBase64Text));
            MemoryStream imageStream = new MemoryStream(Convert.FromBase64String(imageStamp.ImageBase64Text));
            MemoryStream outputStream = new MemoryStream();

            using (var document = PdfDocument.Load(sourceStream))
            {
                var image = PdfImage.Load(imageStream);
                image.Size = new PdfSize(imageStamp.Width, imageStamp.Height);
                foreach (var page in document.Pages)
                {
                    page.Content.DrawImage(image,
                        Stamper.GetCoordinate(page, imageStamp.Height, imageStamp.Width,(Coordinate) imageStamp.PageLocation));
                }

                document.Save(outputStream);
                sourceStream.Dispose();
                imageStream.Dispose();
            }

            //Convert to base64 data
            var outputBase64String = Convert.ToBase64String(outputStream.ToArray());
            var resultObj = new ResultModel()
            {
                ErrorMessages = Array.Empty<string>(), Data = outputBase64String, StatusCode = (int) HttpStatusCode.OK,
                Success = true
            };

            outputStream.Dispose();
            return Results.Ok(resultObj);
        }
        catch (Exception exception)
        {
            var errorMessages = new string[1] {exception.Message};
            return Results.BadRequest(new ResultModel()
            {
                Data = null, ErrorMessages = errorMessages, StatusCode = (int) HttpStatusCode.BadRequest,
                Success = false
            });
        }
    }

    var resultMessages = Stamper.GetValidationMessagesAsArray(validationResults);

    return Results.BadRequest(new ResultModel()
    {
        Data = null, ErrorMessages = resultMessages, StatusCode = (int) HttpStatusCode.UnprocessableEntity,
        Success = false
    });
});

app.Run();

public class ImageStamp
{
    public string FileBase64Text { get; set; }
    public string ImageBase64Text { get; set; }
    public int Height { get; set; } = 16;
    public int Width { get; set; } = 16;
    public int PageLocation { get; set; }
}

public class TextStamp
{
    public string FileBase64String { get; set; }
    public string Text { get; set; }
    public int TextSize { get; set; } = 12;
    public int PageLocation { get; set; }
}