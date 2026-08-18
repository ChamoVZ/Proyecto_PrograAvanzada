-- MathemaX — limpieza de retos duplicados por titulos sin tilde
--
-- Contexto: 2026-07-12_experiencia_y_retos.sql sembraba los mismos retos que
-- MathemaX_Init.sql pero con los titulos sin tilde ("Operador perdido basico"),
-- y como deduplica comparando por [Titulo] nunca reconocia los ya existentes.
-- Resultado: en las bases donde se corrieron los dos scripts hay retos repetidos.
--
-- El origen ya esta corregido en ambos scripts. Esto limpia lo que quedo en la base.
--
-- Se DESACTIVAN en vez de borrarse: algunos duplicados ya tienen partidas jugadas
-- y borrarlos romperia la FK de Partidas y falsearia el marcador. Con Activo = 0
-- dejan de salir en el juego (GetActivosPorModo filtra por esa columna) pero el
-- historial de partidas y el XP ya ganado se conservan.
--
-- Idempotente: correrlo dos veces no cambia nada.

USE MathemaX;
GO

UPDATE Reto
SET    Reto.[Activo] = 0,
       Reto.[LastModified] = GETDATE(),
       Reto.[ModifiedBy] = 'limpieza-duplicados'
FROM [dbo].[Retoes] AS Reto
WHERE Reto.[Activo] = 1
  AND Reto.[Titulo] IN (N'Operador perdido basico',
                        N'Operador perdido medio',
                        N'Operador perdido dificil',
                        N'Secuencia logica simple')
  -- Solo si existe el equivalente con tilde, para no dejar un modo sin retos.
  AND EXISTS (SELECT 1
              FROM [dbo].[Retoes] AS ConTilde
              WHERE ConTilde.[Modo] = Reto.[Modo]
                AND ConTilde.[Enunciado] = Reto.[Enunciado]
                AND ConTilde.[Activo] = 1
                AND ConTilde.[RetoId] <> Reto.[RetoId]);

PRINT 'MathemaX: retos duplicados desactivados.';
GO

-- Verificacion: no deberia quedar ningun titulo sin tilde activo.
SELECT [RetoId], [Modo], [Titulo], [Enunciado], [Activo]
FROM [dbo].[Retoes]
ORDER BY [Modo], [RetoId];
GO
