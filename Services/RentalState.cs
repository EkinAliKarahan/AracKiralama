using AracKiralama.Models;

namespace AracKiralama.Services;

/// <summary>
/// Scoped durum servisi: seçilen araç, tarihler, ek hizmetler ve liste filtresi
/// sayfalar arasında bu servis üzerinden taşınır.
/// </summary>
public class RentalState
{
    private readonly HashSet<string> _extraIds = new();

    public Car? SelectedCar { get; private set; }
    public DateTime PickupDate { get; private set; } = DateTime.Today.AddDays(1);
    public DateTime ReturnDate { get; private set; } = DateTime.Today.AddDays(4);
    public CarFilter Filter { get; private set; } = new();

    public event Action? OnChange;

    public IReadOnlyCollection<string> SelectedExtraIds => _extraIds;
    public IReadOnlyList<ExtraService> SelectedExtras => ExtraCatalog.GetByIds(_extraIds);

    public int RentalDays => RentalRules.CalculateDays(PickupDate, ReturnDate);
    public IReadOnlyList<string> DateErrors => RentalRules.ValidateDates(PickupDate, ReturnDate, DateTime.Today);
    public bool HasValidDates => DateErrors.Count == 0;

    public decimal TotalAmount => SelectedCar is null || !HasValidDates
        ? 0
        : RentalRules.CalculateTotal(RentalDays, SelectedCar.DailyPrice, SelectedExtras);

    public void SelectCar(Car car)
    {
        SelectedCar = car;
        // Önceki oturumdan kalan tarih geçmişte kaldıysa varsayılanlara dön
        if (PickupDate.Date < DateTime.Today)
        {
            PickupDate = DateTime.Today.AddDays(1);
            ReturnDate = DateTime.Today.AddDays(4);
        }
        NotifyStateChanged();
    }

    public void SetPickupDate(DateTime date)
    {
        PickupDate = date.Date;
        NotifyStateChanged();
    }

    public void SetReturnDate(DateTime date)
    {
        ReturnDate = date.Date;
        NotifyStateChanged();
    }

    public void SetExtra(string extraId, bool selected)
    {
        if (selected) _extraIds.Add(extraId); else _extraIds.Remove(extraId);
        NotifyStateChanged();
    }

    public void SetFilter(CarFilter filter)
    {
        Filter = filter;
        NotifyStateChanged();
    }

    /// <summary>Rezervasyon tamamlandıktan sonra ek hizmet seçimini sıfırlar.</summary>
    public void ClearAfterBooking()
    {
        _extraIds.Clear();
        SelectedCar = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
