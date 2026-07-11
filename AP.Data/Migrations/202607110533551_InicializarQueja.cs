namespace AP.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InicializarQueja : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Quejas",
                c => new
                    {
                        QuejaId = c.Int(nullable: false, identity: true),
                        UsuarioId = c.String(nullable: false, maxLength: 128),
                        Asunto = c.String(nullable: false, maxLength: 200),
                        Descripcion = c.String(nullable: false, maxLength: 2000),
                        Categoria = c.Int(nullable: false),
                        Estado = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModified = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QuejaId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Quejas");
        }
    }
}
