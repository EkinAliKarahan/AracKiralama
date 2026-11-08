namespace AracKiralama.Models;

/// <summary>Kiralamaya eklenebilen günlük ücretli ek hizmet.</summary>
public class ExtraService
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal DailyPrice { get; init; }
    public string Icon { get; init; } = "";
}

/// <summary>Sabit ek hizmet kataloğu.</summary>
public static class ExtraCatalog
{
    public static IReadOnlyList<ExtraService> All { get; } = new List<ExtraService>
    {
        new() { Id = "full-insurance", Name = "Tam Sigorta",   DailyPrice = 250, Icon = "shield",
                Description = "Hasar ve hırsızlığa karşı muafiyetsiz tam koruma" },
        new() { Id = "child-seat",     Name = "Çocuk Koltuğu", DailyPrice = 75,  Icon = "baby",
                Description = "0–4 yaş için ISOFIX uyumlu güvenlik koltuğu" },
        new() { Id = "extra-driver",   Name = "Ek Sürücü",     DailyPrice = 150, Icon = "user-plus",
                Description = "Aracı ikinci bir sürücü de yasal olarak kullanabilir" },
    };

    public static IReadOnlyList<ExtraService> GetByIds(IEnumerable<string> ids)
    {
        var set = ids.ToHashSet();
        return All.Where(e => set.Contains(e.Id)).ToList();
    }
}
