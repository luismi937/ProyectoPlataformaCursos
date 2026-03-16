namespace ProyectoPlataformaCursos.Models
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Currency { get; set; } = "eur";
    }
}