# AliCar – Araç Kiralama ve Rezervasyon (Blazor WebAssembly)

## Çalıştırma
```
dotnet run
```
Tarayıcıda `http://localhost:5154` adresini açın.

## Klasör yapısı
```
Models/      Car, CarBooking, CarFilter, CustomerInfo (doğrulama), ExtraService, BookingStatus
Services/    CarService (JSON), BookingService (localStorage), RentalState (scoped durum), RentalRules (iş kuralları)
Components/  CarList, CarCard, CarFilterPanel, CarDetails, RentalOptions, RentalSummary,
             CustomerForm, BookingConfirmation, BookingCard, AlertBox, EmptyState, Icon  (+ her birinin .razor.css'i)
Pages/       Cars (/cars), Rent (/cars/{id}/rent), BookingDetail (/booking/{kod}), MyBookings (/my-bookings)
Layout/      MainLayout, NavMenu
wwwroot/     data/cars.json, images/cars/*.svg, css/app.css
```

## Video için önerilen akış
1. **Kod yapısı:** Models / Services / Components / Pages klasörleri, `Program.cs` içindeki servis kayıtları.
2. **JSON & CarService:** `cars.json` (15 araç, 4 kategori) → `CarService.GetCarsAsync()` (HttpClient).
3. **/cars:** Kategori, marka, yakıt, vites, fiyat aralığı filtreleri; geçersiz fiyat aralığı uyarısı; sıralama;
   müsait olmayan araçlarda pasif "Kirala" butonu.
4. **[Parameter] / EventCallback:** `CarCard → CarList → Cars` ve `RentalOptions → Rent` akışını kodda göster.
5. **/cars/{id}/rent:** Tarih seç (10–13 Kasım = 3 gün), geçmiş tarih ve ters tarih hatalarını göster,
   ek hizmetleri seçince toplamın anlık değiştiğini göster.
6. **Form doğrulama:** Boş gönderim, hatalı e-posta/telefon, koşulları kabul etmeme → hata mesajları;
   `CustomerInfo.cs` öznitelikleri.
7. **Onay:** CAR-2026-101 kodu, araç, tarihler, hizmetler, toplam.
8. **Çakışma:** Aynı araca örtüşen tarihlerle tekrar kiralamayı dene → engellenir.
9. **/my-bookings:** Özet kartları, sekmeler, iptal (onaylı), **F5 ile yenile** → kayıtlar ve iptal durumu korunur;
   tarayıcı DevTools → Application → Local Storage → `alicar.bookings.v1`.
10. **Scoped CSS & responsive:** `.razor.css` dosyaları, DevTools ile mobil görünüm.
