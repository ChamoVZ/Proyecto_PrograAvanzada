using System.Data.Entity;
using System.Data.Common;
using AP.Data.Entities;

namespace AP.Data
{
    /// <summary>Contexto de las entidades de dominio de MathemaX.</summary>
    public class MathemaXContext : DbContext
    {
        public MathemaXContext() : base("name=MathemaXContext")
        {
        }

        public MathemaXContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        public DbSet<Reto> Retos { get; set; }
        public DbSet<Partida> Partidas { get; set; }

        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<SolicitudTI> SolicitudesTI { get; set; }
        public DbSet<Queja> Quejas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
