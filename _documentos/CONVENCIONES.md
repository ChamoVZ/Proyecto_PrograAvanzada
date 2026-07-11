# Convenciones de Código y Nomenclatura — MathemaX

Este documento define las reglas de nomenclatura y estilo de código para mantener la consistencia en el proyecto. Todos los miembros del equipo deben aplicarlas en sus respectivos avances.

## Idioma

- **Español:** Entidades de base de datos, propiedades de dominio y ViewModels. 
  - *Ejemplos:* `Reto`, `Dificultad`, `Partida`, `RetoViewModel`.
- **Inglés:** Clases técnicas, de infraestructura y patrones base.
  - *Ejemplos:* `RepositoryBase`, `EmailService`, `AppException`.

## Clases y Archivos

- **Controllers:** Sustantivo singular + sufijo `Controller`.
  - *Ejemplos:* `RetoController`, `ForoController`.
- **Vistas:** El archivo debe tener exactamente el mismo nombre que la acción que lo renderiza.
  - *Ejemplos:* `Index.cshtml`, `Crear.cshtml`.
- **Vistas Parciales:** Deben comenzar con un guion bajo.
  - *Ejemplos:* `_Sidebar.cshtml`, `_LoginPartial.cshtml`.

## Miembros de Clase

- **Métodos Públicos:** `PascalCase`, deben comenzar con un verbo que indique la acción.
  - *Ejemplos:* `GetRetos()`, `SaveOrUpdate()`, `DeleteReto()`.
- **Campos Privados:** `_camelCase` (empezando con guion bajo).
  - *Ejemplos:* `_repositoryReto`, `_emailService`.
- **Constantes:** `PascalCase`.
  - *Ejemplos:* `MinPasswordLength`, `MaxRetries`.

## Comentarios

- Breves, útiles y en español.
- Deben explicar el "por qué" de una lógica compleja, no reescribir lo que ya es obvio en el código.
- Evitar comentarios generados por herramientas que no aporten valor real.
