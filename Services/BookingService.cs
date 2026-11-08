using System.Text.Json;
using AracKiralama.Helpers;
using AracKiralama.Models;
using Microsoft.JSInterop;

namespace AracKiralama.Services;

public sealed record BookingResult(bool Success, CarBooking? Booking, string? Error, CarBooking? Conflict)
{
    public static BookingResult Ok(CarBooking booking) => new(true, booking, null, null);
    public static BookingResult Fail(string error, CarBooking? conflict = null) => new(false, null, error, conflict);
}

/// <summary>
/// Rezervasyonları tarayıcının localStorage alanında JSON olarak saklar.
/// Her okuma localStorage'dan yapılır; böylece sayfa yenilense de kayıtlar korunur.
/// </summary>
public class BookingService
{
    private const string StorageKey = "alicar.bookings.v1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;

    /// <summary>Rezervasyon eklendiğinde veya iptal edildiğinde tetiklenir (ör. menüdeki rozet).</summary>
    public event Action? OnChange;

    public BookingService(IJSRuntime js) => _js = js;

    public async Task<List<CarBooking>> GetAllAsync()
    {
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(json)) return new();
        try
        {
            return JsonSerializer.Deserialize<List<CarBooking>>(json, JsonOptions) ?? new();
        }
        catch (JsonException)
        {
            return new(); // Bozuk veri: boş liste ile devam et
        }
    }

    public async Task<CarBooking?> GetByIdAsync(string id) =>
        (await GetAllAsync()).FirstOrDefault(b => string.Equals(b.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Bir araç için iptal edilmemiş ve bitmemiş rezervasyonlar.</summary>
    public async Task<List<CarBooking>> GetActiveForCarAsync(string carId) =>
        (await GetAllAsync())
            .Where(b => !b.IsCancelled && b.CarId == carId && b.ReturnDate.Date > DateTime.Today)
            .OrderBy(b => b.PickupDate)
            .ToList();

    public static CarBooking? FindConflict(IEnumerable<CarBooking> bookings, string carId, DateTime pickup, DateTime returnDate) =>
        bookings.FirstOrDefault(b =>
            !b.IsCancelled &&
            b.CarId == carId &&
            RentalRules.Overlaps(pickup, returnDate, b.PickupDate, b.ReturnDate));

    public async Task<BookingResult> CreateAsync(CarBooking booking)
    {
        var errors = RentalRules.ValidateDates(booking.PickupDate, booking.ReturnDate, DateTime.Today);
        if (errors.Count > 0) return BookingResult.Fail(errors[0]);

        var all = await GetAllAsync();

        var conflict = FindConflict(all, booking.CarId, booking.PickupDate, booking.ReturnDate);
        if (conflict is not null)
        {
            return BookingResult.Fail(
                $"Bu araç için {Fmt.Range(conflict.PickupDate, conflict.ReturnDate)} tarihlerinde " +
                $"{conflict.Id} kodlu aktif bir rezervasyon var. Lütfen farklı tarihler seçin.", conflict);
        }

        booking.Id = GenerateCode(all, DateTime.Now.Year);
        booking.CreatedAt = DateTime.Now;
        booking.IsCancelled = false;

        all.Add(booking);
        await SaveAsync(all);
        OnChange?.Invoke();
        return BookingResult.Ok(booking);
    }

    public async Task<bool> CancelAsync(string id)
    {
        var all = await GetAllAsync();
        var booking = all.FirstOrDefault(b => b.Id == id);
        if (booking is null || !booking.CanCancel(DateTime.Today)) return false;

        booking.IsCancelled = true;
        booking.CancelledAt = DateTime.Now;
        await SaveAsync(all);
        OnChange?.Invoke();
        return true;
    }

    private Task SaveAsync(List<CarBooking> bookings) =>
        _js.InvokeVoidAsync("localStorage.setItem", StorageKey, JsonSerializer.Serialize(bookings, JsonOptions)).AsTask();

    /// <summary>Benzersiz kod: CAR-{yıl}-{sıra}. Sıra 101'den başlar ve yıl içinde artar.</summary>
    private static string GenerateCode(IEnumerable<CarBooking> existing, int year)
    {
        var prefix = $"CAR-{year}-";
        var last = existing
            .Where(b => b.Id.StartsWith(prefix, StringComparison.Ordinal))
            .Select(b => int.TryParse(b.Id[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(100)
            .Max();
        return $"{prefix}{last + 1}";
    }
}
