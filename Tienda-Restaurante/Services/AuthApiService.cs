using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Tienda_Restaurante.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _client;

        public AuthApiService(HttpClient client, IConfiguration config)
        {
            _client = client;

            // Configurar la URL base de la API desde appsettings.json
            var baseUrl = config["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new Exception("❌ ApiSettings:BaseUrl no está configurado en appsettings.json");

            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<string?> ObtenerToken(string email, string password)
        {
            var data = new
            {
                email = email,
                password = password
            };

            // Cambié la ruta al endpoint correcto
            var response = await _client.PostAsJsonAsync("Auth/IniciarSesion", data);

            if (!response.IsSuccessStatusCode)
            {
                // Opcional: log para debug
                var contenido = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error API: {contenido}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return result?.Token;
        }


        public class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}