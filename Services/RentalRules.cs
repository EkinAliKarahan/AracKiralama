using AracKiralama.Models;

namespace AracKiralama.Services;

/// <summary>Kiralama iş kuralları: tarih doğrulama, gün sayısı, fiyat ve çakışma hesapları.</summary>
public static class RentalRules
{
    public const int MaxRentalDays = 30;

    /// <summary>Gün sayısı = iade tarihi − teslim tarihi (ör. 10–13 Kasım = 3 gün).</summary>
    public static int CalculateDays(DateTime pickup, DateTime returnDate)
    {
        var days = (returnDate.Date - pickup.Date).Days;
        return days > 0 ? days : 0;
    }

    public static decimal ExtrasDailyTotal(IEnumerable<ExtraService> extras) => extras.Sum(e => e.DailyPrice);

    /// <summary>Toplam = gün × (araç günlük ücreti + seçilen hizmetlerin günlük ücretleri).</summary>
    public static decimal CalculateTotal(int days, decimal carDailyPrice, IEnumerable<ExtraService> extras) =>
        days * (carDailyPrice + ExtrasDailyTotal(extras));

    public static IReadOnlyList<string> ValidateDates(DateTime pickup, DateTime returnDate, DateTime today)
    {
        var errors = new List<string>();
        if (pickup.Date < today.Date)
            errors.Add("Teslim alma tarihi geçmişte olamaz.");
        if (returnDate.Date <= pickup.Date)
            errors.Add("İade tarihi, teslim alma tarihinden sonra olmalıdır.");
        else if ((returnDate.Date - pickup.Date).Days > MaxRentalDays)
            errors.Add($"Tek seferde en fazla {MaxRentalDays} gün kiralama yapılabilir.");
        return errors;
    }

    /// <summary>
    /// İki tarih aralığı çakışıyor mu? İade günü, yeni kiralamanın teslim günü olabilir
    /// (10–13 ve 13–15 Kasım çakışmaz).
    /// </summary>
    public static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd) =>
        aStart.Date < bEnd.Date && bStart.Date < aEnd.Date;
}
