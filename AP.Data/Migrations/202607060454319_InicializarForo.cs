namespace AP.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InicializarForo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Publicacions",
                c => new
                    {
                        PublicacionId = c.Int(nullable: false, identity: true),
                        UsuarioId = c.String(nullable: false, maxLength: 128),
                        Titulo = c.String(nullable: false, maxLength: 200),
                        Contenido = c.String(nullable: false, maxLength: 2000),
                        FechaPublicacion = c.DateTime(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModified = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PublicacionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Publicacions");
        }
    }
}
