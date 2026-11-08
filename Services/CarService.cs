using System.Net.Http.Json;
using AracKiralama.Models;

namespace AracKiralama.Services;

/// <summary>
/// Araç verilerini wwwroot/data/cars.json dosyasından HttpClient ile asenkron yükler.
/// İlk yüklemeden sonra liste bellekte tutulur (scoped = tarayıcı sekmesi boyunca).
/// </summary>
public class CarService
{
    private const string DataPath = "data/cars.json";
    private readonly HttpClient _http;
    private List<Car>? _cache;
    private Task<List<Car>>? _loading;

    public CarService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<Car>> GetCarsAsync()
    {
        if (_cache is not null) return _cache;

        // Aynı anda birden çok bileşen isterse tek bir HTTP isteği yapılır
        _loading ??= LoadAsync();
        try
        {
            _cache = await _loading;
            return _cache;
        }
        catch
        {
            _loading = null; // Hata sonrası tekrar denemeye izin ver
            throw;
        }
    }

    public async Task<Car?> GetCarByIdAsync(string id)
    {
        var cars = await GetCarsAsync();
        return cars.FirstOrDefault(c => string.Equals(c.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<List<Car>> LoadAsync() =>
        await _http.GetFromJsonAsync<List<Car>>(DataPath) ?? new List<Car>();
}
