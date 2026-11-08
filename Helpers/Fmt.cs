using System.Globalization;

namespace AracKiralama.Helpers;

/// <summary>Para ve tarih biçimlendirme yardımcıları (tr-TR).</summary>
public static class Fmt
{
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    public static string Money(decimal value) => value.ToString("N0", Tr) + " ₺";
    public static string Date(DateTime d) => d.ToString("d MMMM yyyy", Tr);
    public static string DateWithDay(DateTime d) => d.ToString("d MMMM yyyy, dddd", Tr);
    public static string ShortDate(DateTime d) => d.ToString("d MMM", Tr);
    public static string Timestamp(DateTime d) => d.ToString("d MMM yyyy HH:mm", Tr);

    public static string Range(DateTime from, DateTime to) =>
        from.Year == to.Year ? $"{ShortDate(from)} – {Date(to)}" : $"{Date(from)} – {Date(to)}";

    /// <summary>input[type=date] için ISO biçimi.</summary>
    public static string ToInputDate(DateTime d) => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static bool TryParseInputDate(string? value, out DateTime date) =>
        System.DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
}
