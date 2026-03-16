using ProyectoPlataformaCursos.Models;
using Stripe;

namespace ProyectoPlataformaCursos.Services
{
    public class StripePaymentService
    {
        private readonly StripeSettings _settings;

        public StripePaymentService(Microsoft.Extensions.Options.IOptions<StripeSettings> options)
        {
            _settings = options.Value;
            StripeConfiguration.ApiKey = _settings.SecretKey;
        }

        public async Task<(bool Success, string Message, string? PaymentIntentId)> CreateAndConfirmPaymentIntentAsync(
            decimal amount,
            string cardHolderName,
            string cardNumber,
            string cardExpiry,
            string cardCvc,
            string description)
        {
            if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            {
                return (false, "Stripe no está configurado (SecretKey vacía)", null);
            }

            if (!TryParseExpiry(cardExpiry, out var expMonth, out var expYear))
            {
                return (false, "La fecha de caducidad de la tarjeta no es válida. Usa MM/AA", null);
            }

            try
            {
                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Number = cardNumber?.Replace(" ", string.Empty),
                        ExpMonth = expMonth,
                        ExpYear = expYear,
                        Cvc = cardCvc
                    },
                    BillingDetails = new PaymentMethodBillingDetailsOptions
                    {
                        Name = cardHolderName
                    }
                });

                var paymentIntentService = new PaymentIntentService();
                var intent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt64(Math.Round(amount * 100m, 0)),
                    Currency = _settings.Currency,
                    PaymentMethod = paymentMethod.Id,
                    Confirm = true,
                    Description = description,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = false
                    }
                });

                if (intent.Status == "succeeded")
                {
                    return (true, "Pago con tarjeta procesado correctamente", intent.Id);
                }

                return (false, $"Pago no completado. Estado Stripe: {intent.Status}", intent.Id);
            }
            catch (StripeException ex)
            {
                return (false, $"Stripe error: {ex.StripeError?.Message ?? ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error procesando pago: {ex.Message}", null);
            }
        }

        private static bool TryParseExpiry(string value, out long month, out long year)
        {
            month = 0;
            year = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var parts = value.Split('/');
            if (parts.Length != 2)
            {
                return false;
            }

            if (!long.TryParse(parts[0].Trim(), out month) || month < 1 || month > 12)
            {
                return false;
            }

            if (!long.TryParse(parts[1].Trim(), out var yearPart))
            {
                return false;
            }

            year = yearPart < 100 ? 2000 + yearPart : yearPart;
            return year >= DateTime.UtcNow.Year;
        }
    }
}