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

## Codificación de archivos

Antes teníamos tres formas distintas de escribir la misma tilde, así que se unificó todo a una sola.

- Todos los archivos de código (`.cs`, `.cshtml`, `.css`, `.sql`) se guardan en **UTF-8 con BOM**. En Visual Studio: *Archivo → Guardar como… → flecha del botón Guardar → Guardar con codificación → Unicode (UTF-8 con firma)*.
- Las tildes y los signos `¿ ¡ ñ` se escriben **literales**, tanto en las vistas Razor como en las cadenas de C#.
- No usar entidades HTML (`&aacute;`) ni escapes Unicode (`ó`) para los acentos. Sí se admiten `&copy;`, `&mdash;`, `&middot;` y `&nbsp;`, que son entidades tipográficas y no parches de codificación.
- Los literales SQL con acentos llevan prefijo `N'...'` para no depender de la collation del servidor.
- El `Web.config` declara `<globalization ... fileEncoding="utf-8" />`. Sin esa línea ASP.NET lee las vistas con el codepage ANSI de Windows y las tildes salen mal. Al copiar `Web.config.example` a `Web.config`, no la borre.

## Comentarios

- Breves, útiles y en español.
- Deben explicar el "por qué" de una lógica compleja, no reescribir lo que ya es obvio en el código.
- Evitar comentarios generados por herramientas que no aporten valor real.
