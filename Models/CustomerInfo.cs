using System.ComponentModel.DataAnnotations;

namespace AracKiralama.Models;

/// <summary>
/// Müşteri formunun doğrulama modeli (EditForm + DataAnnotationsValidator).
/// Boş/boşluk girişleri null'a çevrilir; böylece boş alanda yalnızca [Required] mesajı görünür.
/// </summary>
public class CustomerInfo
{
    private string? _fullName;
    private string? _email;
    private string? _phone;

    [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
    [StringLength(60, MinimumLength = 5, ErrorMessage = "Ad soyad {2}–{1} karakter arasında olmalıdır.")]
    [RegularExpression(@"^\s*[A-Za-zÇĞİÖŞÜçğıöşüÂâÎîÛû]+(?:[ '\-][A-Za-zÇĞİÖŞÜçğıöşüÂâÎîÛû]+)+\s*$",
        ErrorMessage = "Ad ve soyadınızı yalnızca harf kullanarak, aralarında boşluk bırakarak girin.")]
    public string? FullName { get => _fullName; set => _fullName = Normalize(value); }

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin (örn. ad@ornek.com).")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$",
        ErrorMessage = "E-posta adresi geçerli bir alan adı içermelidir (örn. ad@ornek.com).")]
    [StringLength(100, ErrorMessage = "E-posta en fazla {1} karakter olabilir.")]
    public string? Email { get => _email; set => _email = Normalize(value); }

    [Required(ErrorMessage = "Telefon alanı zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [RegularExpression(@"^(\+90\s?|0)?5\d{2}\s?\d{3}\s?\d{2}\s?\d{2}$",
        ErrorMessage = "Cep telefonunu 05XX XXX XX XX biçiminde girin.")]
    [StringLength(17, MinimumLength = 10, ErrorMessage = "Telefon {2}–{1} karakter arasında olmalıdır.")]
    public string? Phone { get => _phone; set => _phone = Normalize(value); }

    [Range(typeof(bool), "true", "true", ErrorMessage = "Devam etmek için kiralama koşullarını kabul etmelisiniz.")]
    public bool AcceptTerms { get; set; }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
