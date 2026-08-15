using System.Net.Http.Json;
using Transacciones.API.DTOs;

namespace Transacciones.API.Clients
{
    public class ProductoRestClient : IProductoRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductoRestClient> _logger;

        public ProductoRestClient(HttpClient httpClient, ILogger<ProductoRestClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductoResponseDto?> GetProductoByIdAsync(int productoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/productos/{productoId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Respuesta no exitosa ({StatusCode}) al consultar producto {Id}", response.StatusCode, productoId);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ProductoResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse de forma síncrona con Productos.API para el producto {Id}", productoId);
                throw new InvalidOperationException($"Error de comunicación síncrona con el microservicio de Productos: {ex.Message}");
            }
        }

        public async Task<bool> AjustarStockAsync(int productoId, int cantidadDelta)
        {
            try
            {
                var request = new AjustarStockRequestDto { CantidadDelta = cantidadDelta };
                var response = await _httpClient.PatchAsJsonAsync($"api/productos/{productoId}/stock", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al ajustar stock ({StatusCode}): {Body}", response.StatusCode, errorBody);

                    string cleanMessage = "No se pudo actualizar el stock en el servicio de productos.";
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(errorBody);
                        if (doc.RootElement.TryGetProperty("mensaje", out var msgElement))
                        {
                            cleanMessage = msgElement.GetString() ?? cleanMessage;
                        }
                        else if (!string.IsNullOrWhiteSpace(errorBody))
                        {
                            cleanMessage = errorBody;
                        }
                    }
                    catch
                    {
                        if (!string.IsNullOrWhiteSpace(errorBody)) cleanMessage = errorBody;
                    }

                    throw new InvalidOperationException(cleanMessage);
                }

                return true;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error síncrono al ajustar stock del producto {Id}", productoId);
                throw new InvalidOperationException($"Fallo en la comunicación síncrona con Productos.API: {ex.Message}");
            }
        }
    }
}
