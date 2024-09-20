using inmobiliariaAST.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using inmobiliariaAST.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Registrar RepositorioUsuario como servicio
builder.Services.AddScoped<RepositorioUsuario>();
builder.Services.AddScoped<AuthenticationService>();

// Configuración de autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Ruta a tu acción de Login
        options.LogoutPath = "/Auth/Logout"; // Ruta a tu acción de Logout
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Ruta a tu página de acceso denegado
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
