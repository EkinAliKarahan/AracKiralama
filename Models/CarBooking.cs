namespace AracKiralama.Models;

/// <summary>Tamamlanmış bir rezervasyon kaydı. localStorage içinde JSON olarak saklanır.</summary>
public class CarBooking
{
    public string Id { get; set; } = "";            // Rezervasyon kodu, örn. CAR-2026-101
    public string CarId { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public List<string> ExtraIds { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    // Ek bilgi alanları (listeleme ve sıralama için)
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}
