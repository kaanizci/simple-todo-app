using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using TodoApi; // Firebase middleware için

var builder = WebApplication.CreateBuilder(args);

// 🔐 Firebase JSON dosyasını environment variable'dan oku
var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CONFIG_JSON");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(firebaseJson)
});

// ✅ CORS AYARI
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://senin-site-adresin.onrender.com") // render adresini ekle
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // ✨ Token taşımak için kritik
    });
});

// 💾 PostgreSQL bağlantısı
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")));

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

app.UseCors();              // 🔴 CORS middleware
app.UseFirebaseAuth();      // 🟡 Firebase kimlik doğrulama
app.UseAuthorization();     // 🔵 Yetkilendirme

app.MapControllers();
app.Run();