using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Application.Services;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorizationBuilder().AddPolicy("UserAccess", policy =>
    policy.RequireRole("User", "Admin"));

builder.Services.AddSingleton(_ =>
{
    var firestore = new FirestoreDbBuilder
    {
        ProjectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID") ?? builder.Configuration["Firebase:ProjectId"]
    };

    var credenciaisJson = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS_JSON");
    var arquivoCredenciais = Path.Combine(builder.Environment.ContentRootPath, "firebase-credentials.json");

    if (!string.IsNullOrEmpty(credenciaisJson))
        firestore.GoogleCredential = CredentialFactory.FromJson<ServiceAccountCredential>(credenciaisJson).ToGoogleCredential();
    else if (File.Exists(arquivoCredenciais))
        firestore.GoogleCredential = CredentialFactory.FromFile<ServiceAccountCredential>(arquivoCredenciais).ToGoogleCredential();

    return firestore.Build();
});

builder.Services.AddScoped<IGastoRepository, GastoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICofrinhoRepository, CofrinhoRepository>();
builder.Services.AddScoped<ICompromissoRepository, CompromissoRepository>();

builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICofrinhoService, CofrinhoService>();
builder.Services.AddScoped<ICompromissoService, CompromissoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Erro");
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
