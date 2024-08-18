namespace AltiKarePDFStampler.Models;

public class ResultModel
{
    public int StatusCode { get; set; }
    public string[] ErrorMessages { get; set; }
    public object Data { get; set; }
    public bool Success { get; set; } = true;
}