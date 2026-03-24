//Referencia para ORM
using Microsoft.EntityFrameworkCore;

namespace ApiBeachSA.Model
{
    public class BeachContext : DbContext
    {
        //Constructor con parámetros que llama al constructor base
        public BeachContext(DbContextOptions<BeachContext> options) :
            base(options)
        {

        }

        //Permite realizar transacciones SQL en cada tabla
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<Reservacion> Reservaciones { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<DetallePago> DetallesPago { get; set; }
    } //Cierre class
} //Cierre namespace
