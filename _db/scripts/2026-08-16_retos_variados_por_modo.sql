IF DB_ID(N'MathemaX') IS NULL
BEGIN
    RAISERROR('No existe la base MathemaX. Primero ejecute _db/MathemaX_Init.sql.', 16, 1);
    RETURN;
END

USE [MathemaX];

IF OBJECT_ID(N'[dbo].[Retoes]', N'U') IS NULL
BEGIN
    RAISERROR('Falta la tabla Retoes. Primero ejecute _db/MathemaX_Init.sql en la base MathemaX.', 16, 1);
    RETURN;
END

-- Hasta ahora Contrarreloj y Secuencias Lógicas tenían un solo reto activo cada uno, así que
-- "Jugar otra vez" repetía siempre el mismo enunciado. Con esto los tres modos quedan con diez
-- retos y cubren las cinco dificultades.
-- Mismo criterio que el script del 12-07: se compara por titulo, así que un titulo repetido
-- no se inserta dos veces y el script se puede correr las veces que haga falta.
INSERT INTO [dbo].[Retoes]
    ([Titulo], [Modo], [Enunciado], [RespuestaCorrecta], [Dificultad],
     [TiempoLimiteSegundos], [Activo], [CreatedAt], [CreatedBy])
SELECT
    Seed.[Titulo], Seed.[Modo], Seed.[Enunciado], Seed.[RespuestaCorrecta],
    Seed.[Dificultad], Seed.[TiempoLimiteSegundos], 1, GETDATE(), 'seed'
FROM (VALUES
    -- Operador Perdido: la respuesta es el signo que falta.
    (N'Operador perdido: suma corta',      1, N'9 _ 4 = 13',      N'+', 1, 30),
    (N'Operador perdido: resta simple',    1, N'20 _ 8 = 12',     N'-', 2, 25),
    (N'Operador perdido: tabla del siete', 1, N'7 _ 8 = 56',      N'*', 3, 25),
    (N'Operador perdido: división exacta', 1, N'144 _ 12 = 12',   N'/', 3, 25),
    (N'Operador perdido: producto alto',   1, N'13 _ 7 = 91',     N'*', 4, 20),
    (N'Operador perdido: dos cifras',      1, N'23 _ 17 = 391',   N'*', 5, 20),
    (N'Operador perdido: cuadrado exacto', 1, N'625 _ 25 = 25',   N'/', 5, 20),

    -- Contrarreloj: la respuesta es el resultado, y el bono premia responder rápido.
    (N'Contrarreloj: suma rápida',         2, N'¿Cuánto es 15 + 27?',              N'42',  1, 20),
    (N'Contrarreloj: tabla del nueve',     2, N'¿Cuánto es 9 × 6?',                N'54',  1, 20),
    (N'Contrarreloj: resta de dos cifras', 2, N'¿Cuánto es 120 - 48?',             N'72',  2, 20),
    (N'Contrarreloj: multiplicar por 5',   2, N'¿Cuánto es 14 × 5?',               N'70',  2, 18),
    (N'Contrarreloj: división entera',     2, N'¿Cuánto es 256 ÷ 8?',              N'32',  3, 18),
    (N'Contrarreloj: producto mediano',    2, N'¿Cuánto es 18 × 13?',              N'234', 4, 15),
    (N'Contrarreloj: porcentaje',          2, N'¿Cuánto es el 35% de 240?',        N'84',  4, 20),
    (N'Contrarreloj: producto grande',     2, N'¿Cuánto es 27 × 34?',              N'918', 5, 15),
    (N'Contrarreloj: raíz cuadrada',       2, N'¿Cuál es la raíz cuadrada de 1156?', N'34', 5, 20),

    -- Secuencias Lógicas: la respuesta es el siguiente número de la serie.
    (N'Secuencia: de tres en tres',        3, N'3, 6, 9, 12, ?',        N'15',  1, 40),
    (N'Secuencia: decenas',                3, N'10, 20, 30, 40, ?',     N'50',  1, 35),
    (N'Secuencia: cuadrados perfectos',    3, N'1, 4, 9, 16, ?',        N'25',  2, 40),
    (N'Secuencia: Fibonacci',              3, N'1, 1, 2, 3, 5, 8, ?',   N'13',  3, 45),
    (N'Secuencia: números oblongos',       3, N'2, 6, 12, 20, 30, ?',   N'42',  3, 45),
    (N'Secuencia: potencias menos uno',    3, N'1, 3, 7, 15, 31, ?',    N'63',  4, 45),
    (N'Secuencia: cuadrados a la baja',    3, N'100, 81, 64, 49, ?',    N'36',  4, 40),
    (N'Secuencia: números primos',         3, N'2, 3, 5, 7, 11, 13, ?', N'17',  5, 50),
    (N'Secuencia: factoriales',            3, N'1, 2, 6, 24, 120, ?',   N'720', 5, 50)
) AS Seed
    ([Titulo], [Modo], [Enunciado], [RespuestaCorrecta], [Dificultad], [TiempoLimiteSegundos])
WHERE NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Retoes] AS Reto
    WHERE Reto.[Titulo] = Seed.[Titulo]
);

PRINT 'MathemaX: retos variados cargados para los tres modos.';
GO
