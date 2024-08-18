using AltiKarePDFStampler.Models;
using FluentValidation.Results;
using GemBox.Pdf;
using GemBox.Pdf.Content;

namespace AltiKarePDFStampler.Utilities;

public sealed class Stamper
{
    static double MarginLeft = 20, MarginTop = 10, MarginRight = 20, MarginBottom = 10;
    public static PdfPoint PdfPoint = new();

    public static PdfPoint GetCoordinate(PdfPage page, double height,double width, Coordinate coordinate)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(height);

        if (coordinate == Coordinate.TopLeft)
            return new PdfPoint(MarginLeft, page.CropBox.Top - MarginTop - height);
        else if (coordinate == Coordinate.TopCenter)
            return new PdfPoint(page.CropBox.Right / 2, page.CropBox.Top - MarginTop - height);
        else if (coordinate == Coordinate.TopRight)
            return new PdfPoint(page.CropBox.Right -width, page.CropBox.Top - MarginTop - height);
        else if (coordinate == Coordinate.BottomLeft)
            return new PdfPoint(MarginLeft, page.CropBox.Bottom + MarginTop + height);
        else if (coordinate == Coordinate.BottomCenter)
            return new PdfPoint(page.CropBox.Right / 2, page.CropBox.Bottom + MarginTop + height);
        else if (coordinate == Coordinate.BottomRight)
            return new PdfPoint(page.CropBox.Right - width,
                page.CropBox.Bottom + MarginTop + height);
        else
        {
            return new PdfPoint(0, 0);
        }
    }

    public static string[] GetValidationMessagesAsArray(ValidationResult validationResults)
    {
        ArgumentNullException.ThrowIfNull(validationResults);
        var resultArr = validationResults.ToDictionary().Values.ToArray();
        var resultMessages = new string[resultArr.Length];

        for (int i = 0; i < resultArr.Length; i++)
        {
            resultMessages[i] = resultArr[i][0];
        }
        return resultMessages;
    }
}