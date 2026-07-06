namespace AP.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InicializarSolicitudTI : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SolicitudTIs",
                c => new
                    {
                        SolicitudTIId = c.Int(nullable: false, identity: true),
                        UsuarioId = c.String(nullable: false, maxLength: 128),
                        Asunto = c.String(nullable: false, maxLength: 200),
                        Descripcion = c.String(nullable: false, maxLength: 2000),
                        Estado = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModified = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SolicitudTIId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SolicitudTIs");
        }
    }
}
