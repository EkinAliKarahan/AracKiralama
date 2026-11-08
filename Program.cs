using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AracKiralama;
using AracKiralama.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// cars.json dosyasını wwwroot'tan okumak için HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Uygulama servisleri
builder.Services.AddScoped<CarService>();      // Araç verisini JSON'dan asenkron yükler
builder.Services.AddScoped<BookingService>();  // Rezervasyonları localStorage'da saklar
builder.Services.AddScoped<RentalState>();     // Sayfalar arası seçim durumu (araç, tarih, ek hizmet, filtre)

// Para ve tarih biçimleri Türkçe olsun
var culture = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await builder.Build().RunAsync();
