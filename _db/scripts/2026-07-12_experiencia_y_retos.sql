IF DB_ID(N'MathemaX') IS NULL
BEGIN
    RAISERROR('No existe la base MathemaX. Primero ejecute _db/MathemaX_Init.sql.', 16, 1);
    RETURN;
END

USE [MathemaX];

IF OBJECT_ID(N'[dbo].[AspNetUsers]', N'U') IS NULL
    OR OBJECT_ID(N'[dbo].[Retoes]', N'U') IS NULL
BEGIN
    RAISERROR('Faltan tablas base de MathemaX. Primero ejecute _db/MathemaX_Init.sql en la base MathemaX.', 16, 1);
    RETURN;
END

-- Las columnas se agregan por script porque Identity no usa inicializador de EF.
IF COL_LENGTH('dbo.AspNetUsers', 'ExperienciaTotal') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
        ADD [ExperienciaTotal] INT NOT NULL
            CONSTRAINT [DF_AspNetUsers_ExperienciaTotal] DEFAULT (0) WITH VALUES;
END

IF COL_LENGTH('dbo.AspNetUsers', 'Nivel') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
        ADD [Nivel] INT NOT NULL
            CONSTRAINT [DF_AspNetUsers_Nivel] DEFAULT (1) WITH VALUES;
END

-- Cada reto se valida por titulo para completar seeds parciales sin duplicarlos.
INSERT INTO [dbo].[Retoes]
    ([Titulo], [Modo], [Enunciado], [RespuestaCorrecta], [Dificultad],
     [TiempoLimiteSegundos], [Activo], [CreatedAt], [CreatedBy])
SELECT
    Seed.[Titulo], Seed.[Modo], Seed.[Enunciado], Seed.[RespuestaCorrecta],
    Seed.[Dificultad], Seed.[TiempoLimiteSegundos], 1, GETDATE(), 'seed'
FROM (VALUES
    (N'Operador perdido basico',  1, N'4 _ 3 = 12',       N'*',  1, 30),
    (N'Operador perdido medio',   1, N'15 _ 3 = 5',       N'/',  2, 25),
    (N'Contrarreloj: suma',       2, N'Cuanto es 47+36?', N'83', 3, 20),
    (N'Secuencia logica simple',  3, N'2, 4, 8, 16, ?',   N'32', 2, 40),
    (N'Operador perdido dificil', 1, N'81 _ 9 = 9',       N'/',  4, 20)
) AS Seed
    ([Titulo], [Modo], [Enunciado], [RespuestaCorrecta], [Dificultad], [TiempoLimiteSegundos])
WHERE NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Retoes] AS Reto
    WHERE Reto.[Titulo] = Seed.[Titulo]
);

PRINT 'MathemaX: experiencia y retos verificados correctamente.';
GO


USE MathemaX;

SELECT name
FROM sys.tables
WHERE name IN ('AspNetUsers', 'Retoes');

SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers'
  AND COLUMN_NAME IN ('ExperienciaTotal', 'Nivel');

SELECT Titulo
FROM dbo.Retoes
WHERE Titulo LIKE 'Operador perdido%'
   OR Titulo IN ('Contrarreloj: suma', 'Secuencia logica simple');