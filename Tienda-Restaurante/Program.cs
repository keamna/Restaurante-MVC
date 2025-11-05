using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;
using Tienda_Restaurante.Views.Shared;
using Stripe;
using Tienda_Restaurante.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

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

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Map(
        keyPropertyName: "SourceContext",
        defaultKey: "general",
        configure: (sourceContext, writeTo) =>
        {
            var fileName = (sourceContext ?? "general").ToString().Trim('"');
            var lastSegment = fileName.Split('.').LastOrDefault() ?? "general";
            var path = $@"C:\Logs\Proyecto\{lastSegment.ToLower()}.txt";

            writeTo.File(
                path: path,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {SourceContext} | {Level:u3} | {Message:lj}{NewLine}{Exception}"
            );
        })
    .WriteTo.File(
        path: @"C:\Logs\Proyecto\general.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {SourceContext} | {Level:u3} | {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog(); 
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
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddTransient<ICuerpoCorreoService, CuerpoCorreoService>();


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
