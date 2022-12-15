using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;

namespace BusinessEntity
{
    public class Reserva
    {


        public async Task PagarReserva()
        {
            MercadoPagoConfig.AccessToken = "TEST-2506547227462069-121422-0da3758844748756544c9da5a46f94be-109497359";
            var request = new PreferenceRequest
            {

                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://www.tu-sitio/success",
                    Failure = "http://www.tu-sitio/failure",
                    Pending = "http://www.tu-sitio/pendings",
                },
                AutoReturn = "approved",
                Items = new List<PreferenceItemRequest>
    {
        new PreferenceItemRequest
        {
            Title = "Reserva de turno",
            Quantity = 1,
            CurrencyId = "ARS",
            UnitPrice = 3000,
        },
    }
            };


            // Crea la preferencia usando el client
            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);
        }




    }
}