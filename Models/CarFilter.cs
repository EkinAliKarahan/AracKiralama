namespace AracKiralama.Models;

/// <summary>Araç listesi filtre ve sıralama ölçütleri.</summary>
public class CarFilter
{
    public string Brand { get; set; } = "";
    public string Category { get; set; } = "";
    public string FuelType { get; set; } = "";
    public string Transmission { get; set; } = "";
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool OnlyAvailable { get; set; }
    public string SortBy { get; set; } = "recommended";

    public bool HasInvalidPriceRange => MinPrice is decimal min && MaxPrice is decimal max && min > max;

    /// <summary>Aktif (varsayılandan farklı) filtre sayısı. Sıralama dahil değildir.</summary>
    public int ActiveCount =>
        (Brand != "" ? 1 : 0) + (Category != "" ? 1 : 0) + (FuelType != "" ? 1 : 0) +
        (Transmission != "" ? 1 : 0) + (MinPrice.HasValue ? 1 : 0) + (MaxPrice.HasValue ? 1 : 0) +
        (OnlyAvailable ? 1 : 0);

    /// <summary>Değiştirilmiş bir kopya üretir (bileşenler arasında değişmez veri akışı için).</summary>
    public CarFilter With(Action<CarFilter> change)
    {
        var copy = (CarFilter)MemberwiseClone();
        change(copy);
        return copy;
    }

    public IEnumerable<Car> Apply(IEnumerable<Car> cars)
    {
        var q = cars;
        if (Brand != "") q = q.Where(c => c.Brand == Brand);
        if (Category != "") q = q.Where(c => c.Category == Category);
        if (FuelType != "") q = q.Where(c => c.FuelType == FuelType);
        if (Transmission != "") q = q.Where(c => c.Transmission == Transmission);
        if (MinPrice is decimal min) q = q.Where(c => c.DailyPrice >= min);
        if (MaxPrice is decimal max) q = q.Where(c => c.DailyPrice <= max);
        if (OnlyAvailable) q = q.Where(c => c.IsAvailable);

        return SortBy switch
        {
            "price-asc" => q.OrderBy(c => c.DailyPrice),
            "price-desc" => q.OrderByDescending(c => c.DailyPrice),
            "name" => q.OrderBy(c => c.Brand).ThenBy(c => c.Model),
            _ => q.OrderByDescending(c => c.IsAvailable).ThenBy(c => c.DailyPrice), // önerilen
        };
    }
}
