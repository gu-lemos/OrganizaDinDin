using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Application.Services;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuração de Autenticação
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Configuração do Firestore
builder.Services.AddSingleton(provider =>
{
    var credentialsJson = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON");
    var projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID") ?? "organizadindin-ee3b1";

    if (!string.IsNullOrEmpty(credentialsJson))
    {
        var credentialsPath = Path.Combine(Path.GetTempPath(), "firebase-credentials.json");
        File.WriteAllText(credentialsPath, credentialsJson);
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
    }
    else
    {
        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            "firebase-credentials.json");
    }

    return FirestoreDb.Create(projectId);
});

// Injeção de Dependência - Camada de Infraestrutura
builder.Services.AddScoped<IGastoRepository, GastoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Injeção de Dependência - Camada de Aplicação
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
