-- MathemaX — Script de inicialización de base de datos
-- ASP.NET MVC 5 + Entity Framework 6 + ASP.NET Identity 2
-- Ejecutar en SQL Server Management Studio conectado a localhost
-- Fecha: 2026-06-21

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MathemaX')
BEGIN
    CREATE DATABASE [MathemaX];
END
GO

USE [MathemaX];
GO

-- ============================================================
-- TABLAS DE ASP.NET IDENTITY 2
-- ============================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AspNetRoles] (
        [Id]   NVARCHAR (128) NOT NULL,
        [Name] NVARCHAR (256) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
        ON [dbo].[AspNetRoles]([Name] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AspNetUsers] (
        [Id]                   NVARCHAR (128) NOT NULL,
        [Email]                NVARCHAR (256) NULL,
        [EmailConfirmed]       BIT            NOT NULL,
        [PasswordHash]         NVARCHAR (MAX) NULL,
        [SecurityStamp]        NVARCHAR (MAX) NULL,
        [PhoneNumber]          NVARCHAR (MAX) NULL,
        [PhoneNumberConfirmed] BIT            NOT NULL,
        [TwoFactorEnabled]     BIT            NOT NULL,
        [LockoutEndDateUtc]    DATETIME       NULL,
        [LockoutEnabled]       BIT            NOT NULL,
        [AccessFailedCount]    INT            NOT NULL,
        [UserName]             NVARCHAR (256) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
        ON [dbo].[AspNetUsers]([UserName] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles] (
        [UserId] NVARCHAR (128) NOT NULL,
        [RoleId] NVARCHAR (128) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
            FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]([UserId] ASC);
    CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AspNetUserClaims] (
        [Id]         INT            IDENTITY (1, 1) NOT NULL,
        [UserId]     NVARCHAR (128) NOT NULL,
        [ClaimType]  NVARCHAR (MAX) NULL,
        [ClaimValue] NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]([UserId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AspNetUserLogins] (
        [LoginProvider] NVARCHAR (128) NOT NULL,
        [ProviderKey]   NVARCHAR (128) NOT NULL,
        [UserId]        NVARCHAR (128) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
        CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]([UserId] ASC);
END
GO

-- ============================================================
-- HISTORIAL DE MIGRACIONES DE ENTITY FRAMEWORK
-- ============================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[__MigrationHistory] (
        [MigrationId]    NVARCHAR (150) NOT NULL,
        [ContextKey]     NVARCHAR (300) NOT NULL,
        [Model]          VARBINARY (MAX) NOT NULL,
        [ProductVersion] NVARCHAR (32)  NOT NULL,
        CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC, [ContextKey] ASC)
    );
END
GO

-- ============================================================
-- TABLAS DE APLICACION
-- ============================================================

-- Retos: ejercicios matemáticos que el jugador resuelve
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Retoes]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Retoes] (
        [RetoId]               INT            IDENTITY (1, 1) NOT NULL,
        [Titulo]               NVARCHAR (120) NOT NULL,
        -- Modo: 1=OperadorPerdido, 2=Contrarreloj, 3=SecuenciasLogicas
        [Modo]                 INT            NOT NULL,
        [Enunciado]            NVARCHAR (500) NOT NULL,
        [RespuestaCorrecta]    NVARCHAR (100) NOT NULL,
        -- Dificultad entre 1 y 5; determina el XP base
        [Dificultad]           INT            NOT NULL,
        [TiempoLimiteSegundos] INT            NOT NULL,
        [Activo]               BIT            NOT NULL DEFAULT (1),
        [CreatedAt]            DATETIME       NOT NULL DEFAULT (GETDATE()),
        [CreatedBy]            NVARCHAR (MAX) NULL,
        [LastModified]         DATETIME       NULL,
        [ModifiedBy]           NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_dbo.Retoes]       PRIMARY KEY CLUSTERED ([RetoId] ASC),
        CONSTRAINT [CK_Retoes_Dificultad] CHECK ([Dificultad] BETWEEN 1 AND 5)
    );
END
GO

-- Partidas: historial de intentos por usuario (alimenta marcadores)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Partidas]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Partidas] (
        [PartidaId]             INT            IDENTITY (1, 1) NOT NULL,
        -- FK a AspNetUsers.Id (Identity usa NVARCHAR 128)
        [UsuarioId]             NVARCHAR (128) NOT NULL,
        [RetoId]                INT            NOT NULL,
        [Acertado]              BIT            NOT NULL,
        [TiempoEmpleadoSegundos] INT           NOT NULL,
        -- XP calculado en AP.Core, no en la BD
        [XpGanado]              INT            NOT NULL,
        [FechaJuego]            DATETIME       NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_dbo.Partidas]  PRIMARY KEY CLUSTERED ([PartidaId] ASC),
        CONSTRAINT [FK_dbo.Partidas_dbo.Retoes_RetoId]
            FOREIGN KEY ([RetoId]) REFERENCES [dbo].[Retoes] ([RetoId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_RetoId] ON [dbo].[Partidas]([RetoId] ASC);
END
GO

-- ============================================================
-- DATOS INICIALES
-- ============================================================

-- Roles del sistema
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = 'Admin')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name])
    VALUES
        (NEWID(), 'Admin'),
        (NEWID(), 'Player'),
        (NEWID(), 'Support');
END
GO

-- Retos de ejemplo para demostrar el modelo
IF NOT EXISTS (SELECT 1 FROM [dbo].[Retoes])
BEGIN
    INSERT INTO [dbo].[Retoes]
        ([Titulo], [Modo], [Enunciado], [RespuestaCorrecta], [Dificultad], [TiempoLimiteSegundos], [Activo], [CreatedAt], [CreatedBy])
    VALUES
        ('Operador perdido básico',  1, '4 _ 3 = 12',        '*', 1, 30,  1, GETDATE(), 'seed'),
        ('Operador perdido medio',   1, '15 _ 3 = 5',        '/', 2, 25,  1, GETDATE(), 'seed'),
        ('Contrarreloj: suma',       2, '¿Cuánto es 47+36?', '83',3, 20,  1, GETDATE(), 'seed'),
        ('Secuencia lógica simple',  3, '2, 4, 8, 16, ?',    '32',2, 40,  1, GETDATE(), 'seed'),
        ('Operador perdido difícil', 1, '81 _ 9 = 9',        '/', 4, 20,  1, GETDATE(), 'seed');
END
GO

-- Fin del script
PRINT 'MathemaX: base de datos inicializada correctamente.';
GO
