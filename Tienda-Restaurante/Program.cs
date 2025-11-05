using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;
using Tienda_Restaurante.Repositories;
using Tienda_Restaurante.Views.Shared;
using Stripe; 

var builder = WebApplication.CreateBuilder(args);

// Conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de Identity
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

// Repositorios
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IFileService, Tienda_Restaurante.Views.Shared.FileService>();
builder.Services.AddTransient<IPlatilloRepository, PlatilloRepository>();


StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.MapRazorPages();

app.Run();
