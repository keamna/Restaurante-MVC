using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Models;

namespace Tienda_Restaurante.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Platillo> Platillos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<DetalleCarrito> DetallesCarrito { get; set; }
    public DbSet<Orden> Ordenes { get; set; }
    public DbSet<DetalleOrden> DetalleOrdenes { get; set; }
    public DbSet<OrdenEstado> OrdenesEstado { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Reserva> Reservas { get; set; }


}
