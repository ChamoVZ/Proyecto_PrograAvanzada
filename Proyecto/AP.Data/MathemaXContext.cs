using System.Data.Entity;
using AP.Data.Entities;

namespace AP.Data
{
    /// <summary>
    /// DbContext principal de MathemaX (Code First).
    /// Es la ÚNICA puerta hacia SQL Server: ninguna otra capa abre conexiones.
    /// La cadena de conexión "MathemaXContext" vive en el Web.config de AP.MVC
    /// (y en el App.config de AP.Tests si se necesita).
    /// </summary>
    public class MathemaXContext : DbContext
    {
        public MathemaXContext() : base("name=MathemaXContext")
        {
        }

        // Un DbSet por entidad. Agregar aquí cada tabla nueva del dominio.
        public DbSet<Reto> Retos { get; set; }
        public DbSet<Partida> Partidas { get; set; }

        // Pendientes (se agregan conforme avancen los módulos):
        // public DbSet<Insignia> Insignias { get; set; }
        // public DbSet<Publicacion> Publicaciones { get; set; }      // Foro
        // public DbSet<SolicitudTI> SolicitudesTI { get; set; }      // Request IT
        // public DbSet<Queja> Quejas { get; set; }                   // Buzón de Quejas
        // public DbSet<Amistad> Amistades { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuraciones Fluent API (nombres de tabla, relaciones, índices).
            // Ejemplo: forzar nombre de tabla en singular o plural consistente:
            // modelBuilder.Entity<Reto>().ToTable("Retos");
            base.OnModelCreating(modelBuilder);
        }
    }
}
