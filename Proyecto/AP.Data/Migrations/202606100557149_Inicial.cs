namespace AP.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Partidas",
                c => new
                    {
                        PartidaId = c.Int(nullable: false, identity: true),
                        UsuarioId = c.String(nullable: false, maxLength: 128),
                        RetoId = c.Int(nullable: false),
                        Acertado = c.Boolean(nullable: false),
                        TiempoEmpleadoSegundos = c.Int(nullable: false),
                        XpGanado = c.Int(nullable: false),
                        FechaJuego = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PartidaId)
                .ForeignKey("dbo.Retoes", t => t.RetoId, cascadeDelete: true)
                .Index(t => t.RetoId);
            
            CreateTable(
                "dbo.Retoes",
                c => new
                    {
                        RetoId = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 120),
                        Modo = c.Int(nullable: false),
                        Enunciado = c.String(nullable: false, maxLength: 500),
                        RespuestaCorrecta = c.String(nullable: false, maxLength: 100),
                        Dificultad = c.Int(nullable: false),
                        TiempoLimiteSegundos = c.Int(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModified = c.DateTime(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RetoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partidas", "RetoId", "dbo.Retoes");
            DropIndex("dbo.Partidas", new[] { "RetoId" });
            DropTable("dbo.Retoes");
            DropTable("dbo.Partidas");
        }
    }
}
