-- MathemaX — columna Activo para el borrado lógico de Quejas y SolicitudTIs
--
-- Los dos módulos pasan a permitir que el autor retire lo suyo. Igual que en el foro,
-- el registro no se borra: se marca Activo = 0 y deja de listarse, para no perder el
-- historial de lo que soporte ya atendió.
--
-- Los registros existentes quedan activos por el DEFAULT.
-- Idempotente: correrlo dos veces no cambia nada.

IF DB_ID(N'MathemaX') IS NULL
BEGIN
    RAISERROR('No existe la base MathemaX. Primero ejecute _db/MathemaX_Init.sql.', 16, 1);
    RETURN;
END

USE [MathemaX];

IF OBJECT_ID(N'[dbo].[Quejas]', N'U') IS NULL
    OR OBJECT_ID(N'[dbo].[SolicitudTIs]', N'U') IS NULL
BEGIN
    RAISERROR('Faltan tablas base de MathemaX. Primero ejecute _db/MathemaX_Init.sql en la base MathemaX.', 16, 1);
    RETURN;
END

IF COL_LENGTH('dbo.Quejas', 'Activo') IS NULL
BEGIN
    ALTER TABLE [dbo].[Quejas]
        ADD [Activo] BIT NOT NULL
            CONSTRAINT [DF_Quejas_Activo] DEFAULT (1) WITH VALUES;
END

IF COL_LENGTH('dbo.SolicitudTIs', 'Activo') IS NULL
BEGIN
    ALTER TABLE [dbo].[SolicitudTIs]
        ADD [Activo] BIT NOT NULL
            CONSTRAINT [DF_SolicitudTIs_Activo] DEFAULT (1) WITH VALUES;
END

PRINT 'MathemaX: columna Activo lista en Quejas y SolicitudTIs.';
