namespace AracKiralama.Models;

public enum BookingStatus
{
    Upcoming,   // Henüz başlamamış (iptal edilebilir)
    Ongoing,    // Teslim alındı, iade bekleniyor
    Completed,  // İade tarihi geçti
    Cancelled,  // Kullanıcı iptal etti
}

public static class BookingStatusExtensions
{
    public static BookingStatus GetStatus(this CarBooking b, DateTime today)
    {
        if (b.IsCancelled) return BookingStatus.Cancelled;
        if (b.PickupDate.Date > today.Date) return BookingStatus.Upcoming;
        if (b.ReturnDate.Date > today.Date) return BookingStatus.Ongoing;
        return BookingStatus.Completed;
    }

    /// <summary>Sadece henüz başlamamış (teslim tarihi gelmemiş) aktif kiralamalar iptal edilebilir.</summary>
    public static bool CanCancel(this CarBooking b, DateTime today) =>
        !b.IsCancelled && b.PickupDate.Date > today.Date;

    public static string ToLabel(this BookingStatus s) => s switch
    {
        BookingStatus.Upcoming => "Yaklaşan",
        BookingStatus.Ongoing => "Devam ediyor",
        BookingStatus.Completed => "Tamamlandı",
        BookingStatus.Cancelled => "İptal edildi",
        _ => ""
    };

    public static string ToCss(this BookingStatus s) => s.ToString().ToLowerInvariant();
}
