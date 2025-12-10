namespace ConvertidorMonedas.App.Models
{
    public class ConversionModel
    {
        public decimal Amount { get; set; }
        public string? SourceCurrency { get; set; }
        public string? TargetCurrency { get; set; }
        public decimal Result { get; set; }
    }
}
