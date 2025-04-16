using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TFG.Data;
using TFG.Models;
using TFG.Repositories;
using TFG.Services;

var builder = WebApplication.CreateBuilder(args);

//Asignacion de la Cadena de Conexión
builder.Services.AddDbContext<TFGContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Cadena de Conexión 'DevelopmentConnection' no encontrada")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioUsuarios,RepositorioUsuarios>();
builder.Services.AddTransient<IUsuario, Usuario>();
builder.Services.AddTransient<IUserStore<Usuario> , PersistenciaUsuario>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<SignInManager<Usuario>>();

builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    opciones.Password.RequireDigit = false; //NO Requiere Numeros
    opciones.Password.RequireLowercase = false; //NO Requiere minusculas
    opciones.Password.RequireUppercase = false; //NO Requiere mayusculas
    opciones.Password.RequireNonAlphanumeric = false; //NO Requerir alfanumérico
}).AddErrorDescriber<MensajesDeErrorIdentity>();

//Configuracion del servicio de autentificacion de la Coockie
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
