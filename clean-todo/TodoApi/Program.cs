using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using TodoApi; // Firebase middleware için

var builder = WebApplication.CreateBuilder(args);

// 🔐 Firebase Admin SDK başlatılıyor
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("todoapp-241d7-firebase-adminsdk-fbsvc-4969c866ec.json")
});

// ✅ CORS AYARI
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // ✨ Token taşımak için kritik
    });
});

// 💾 PostgreSQL bağlantısı
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=todoapp;Username=postgres;Password=yourpassword"));

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

app.UseCors();              // 🔴 CORS middleware'i burada aktif edilmeli
app.UseFirebaseAuth();      // 🟡 Auth middleware sonra gelir
app.UseAuthorization();     // 🔵 Yetkilendirme (en son değil, ortada)

app.MapControllers();
app.Run();