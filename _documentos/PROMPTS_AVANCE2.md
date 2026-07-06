# Prompts para el Avance 2 — MathemaX

Prompts listos para copiar y pegar en Claude Code o Antigravity IDE.
Cada prompt es autocontenido: pegar primero el BLOQUE DE CONTEXTO y despues el prompt del modulo.

**Orden recomendado de ejecucion:**

1. Auditoria inicial y convenciones de nomenclatura
2. Roles de Identity (Admin, Player, Support)
3. Conexion de capas + CRUD de Retos (mantenimiento admin)
4. Landing page / Home
5. Modulo Foro (estructura inicial)
6. Modulo Solicitud TI (Request IT)
7. Modulo Buzon de Quejas
8. Comentarios SOLID, navegacion y cierre del avance

Antes de empezar cualquier prompt, verificar en la terminal:

```bash
git branch --show-current   # debe decir: dev/sajmzrs
git pull origin dev/sajmzrs
git pull origin staging
```

---

## BLOQUE DE CONTEXTO (pegar al inicio de CADA prompt)

```
CONTEXTO DEL PROYECTO — leer antes de tocar cualquier archivo:

Este es MathemaX, un proyecto universitario del curso SC-601 Programacion Avanzada,
desarrollado por 4 estudiantes. Es una plataforma web de retos matematicos (Math Riddles).

Stack: ASP.NET MVC 5 / .NET Framework 4.8, Entity Framework 6 Code First,
ASP.NET Identity 2, SQL Server Express, Bootstrap 5, jQuery 3.7.

Arquitectura N-capas (las dependencias fluyen hacia abajo, nunca al reves):
  AP.MVC (controllers, vistas Razor, Identity)
    -> AP.Core (logica de negocio, AppException, GlobalValidation)
    -> AP.Repositories (IRepositoryBase<T> generico + repositorios concretos)
    -> AP.Data (entidades EF, MathemaXContext, migraciones)
  AP.Models (ViewModels/DTOs, es lo UNICO que llega a las vistas)
  AP.Services (servicios auxiliares: EmailService, ChatbotService)
  AP.Tests (MSTest)

Documentos de referencia dentro del repo (leerlos primero):
  BITACORA.md               -> estado del proyecto y alcance de cada avance
  README.md                 -> estructura y setup
  _documentos/FLUJO_GIT.md  -> flujo de ramas y convenciones de commit
  _documentos/DISENO.md     -> paleta de colores y tipografias
  _db/MathemaX_Init.sql     -> esquema de base de datos

REGLAS OBLIGATORIAS PARA TODO EL TRABAJO:

1. RAMA: antes de modificar cualquier archivo, ejecuta git branch --show-current
   y verifica que la rama activa sea dev/sajmzrs. Si no lo es, DETENTE y avisa.
   No cambies de rama por tu cuenta ni trabajes en main, staging o production.

2. SIN COMMITS: no ejecutes git add, git commit ni git push en ningun momento.
   Solo modifica archivos. Yo reviso los cambios y decido cuando hacer commit.

3. ANALISIS PREVIO: antes de escribir codigo nuevo, lee el codigo existente
   relacionado con la tarea. Valida que lo ya implementado cumple con lo que
   dice la BITACORA para el Avance 1 y sigue la arquitectura descrita arriba.
   Si encuentras algo que no cumple, repórtalo primero y propon la correccion
   antes de aplicarla. No reescribas codigo que ya funciona sin justificarlo.

4. ESTILO DE ESTUDIANTE: el codigo debe verse como el trabajo de estudiantes
   universitarios aplicados, no como codigo generado por una IA ni codigo
   enterprise sobre-disenado. Eso significa:
   - Comentarios en espanol, breves y utiles, explicando el "por que",
     al estilo de los comentarios que ya existen en el proyecto.
   - Sin patrones innecesarios (no meter AutoMapper, DI containers,
     CQRS ni nada que el curso no pide).
   - Soluciones directas y legibles antes que abstracciones elegantes.

5. CODIGO LIMPIO: nombres descriptivos en espanol o ingles consistente con lo
   existente (entidades y propiedades en espanol como Reto, Dificultad;
   clases tecnicas en ingles como RepositoryBase). Sin codigo muerto, sin
   usings sin usar, sin regiones vacias.

6. SIN EMOJIS: prohibido usar emojis en codigo, comentarios, vistas, mensajes
   al usuario o cualquier archivo del proyecto.

7. RESPETAR LA ARQUITECTURA: los controllers nunca acceden a MathemaXContext
   ni a los repositorios directamente; siempre pasan por AP.Core. Las
   entidades de EF nunca llegan a las vistas; siempre se mapean a ViewModels
   de AP.Models. Las reglas de negocio viven en AP.Core, no en controllers.

8. AL TERMINAR: entrega un resumen de archivos creados/modificados y una
   lista de verificaciones manuales que debo hacer en Visual Studio
   (compilar, correr, que pantallas revisar). No marques nada en BITACORA.md
   a menos que el prompt lo pida explicitamente.
```

---

## PROMPT 1 — Auditoria inicial y convenciones de nomenclatura

```
TAREA: Auditoria del estado actual del proyecto y aplicacion de convenciones
de nomenclatura (primer punto del Avance 2 en BITACORA.md).

PASO 1 — AUDITORIA (solo lectura, no modifiques nada todavia):
Recorre la solucion completa (AP.Data, AP.Repositories, AP.Core, AP.Services,
AP.Models, AP.MVC, AP.Tests) y genera un informe corto que responda:
  a) Que items del Avance 1 de BITACORA.md estan realmente implementados
     y cuales estan a medias o solo declarados.
  b) Donde hay inconsistencias de nomenclatura: mezcla de idiomas sin criterio,
     nombres genericos de la plantilla de Visual Studio (por ejemplo textos
     "Your application description page" en HomeController), archivos de la
     plantilla que ya no aplican, ViewBag sin uso, etc.
  c) Si alguna capa se salta la arquitectura (controller tocando datos, etc.).
Presenta el informe y espera mi confirmacion antes del paso 2.

PASO 2 — APLICAR CONVENCIONES (despues de mi confirmacion):
Define y aplica estas convenciones en todo el codigo del equipo (no tocar
archivos de librerias en Scripts/ ni Content/ de Bootstrap/jQuery):
  - Entidades, propiedades de dominio y ViewModels: espanol (Reto, Dificultad).
  - Clases tecnicas e infraestructura: ingles (RepositoryBase, EmailService).
  - Controllers: sustantivo singular + Controller (RetoController, ForoController).
  - Vistas: mismo nombre de la accion; parciales con guion bajo (_Sidebar).
  - Metodos publicos: PascalCase, verbo primero (GetRetos, SaveOrUpdate).
  - Campos privados: _camelCase (como _repositoryReto ya existente).
  - Constantes: PascalCase (como MinPasswordLength ya existente).
Documenta las convenciones en un archivo nuevo _documentos/CONVENCIONES.md
(corto, media pagina, estilo del resto de documentos del repo).
Limpia los textos placeholder de la plantilla ASP.NET que sigan visibles
(About/Contact de HomeController y sus vistas se veran en el prompt de Home,
aqui solo renombra/limpia lo que sea nomenclatura).

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, comentarios de estudiante.
```

---

## PROMPT 2 — Roles de Identity (Admin, Player, Support)

```
TAREA: Configurar los tres roles de la aplicacion con ASP.NET Identity 2:
Admin, Player y Support (punto "Roles de Identity configurados" del Avance 2).

PASO 1 — ANALISIS PREVIO:
Lee AP.MVC/App_Start/IdentityConfig.cs, Startup.Auth.cs, IdentityModels.cs,
AccountController.cs y AP.Data/Migrations/Configuration.cs. Lee tambien
_db/MathemaX_Init.sql para ver como estan definidas las tablas AspNetRoles y
AspNetUserRoles. Confirma que el login y registro actuales funcionan sobre
Identity 2 tal como dice la BITACORA del Avance 1. Reporta lo que encuentres
antes de modificar.

PASO 2 — IMPLEMENTACION:
  a) Crear los roles Admin, Player y Support de forma idempotente al arrancar
     la aplicacion (si no existen, se crean). Usar RoleManager con el patron
     mas simple posible coherente con IdentityConfig.cs existente.
  b) Al registrarse un usuario nuevo, asignarle automaticamente el rol Player.
  c) Agregar al script _db/MathemaX_Init.sql los INSERT idempotentes de los
     tres roles, siguiendo el estilo IF NOT EXISTS del resto del script.
  d) Dejar preparado un usuario admin de ejemplo SOLO documentado en
     comentario del script (no hardcodear contrasenas en codigo C#).
  e) Comentar en el codigo, estilo estudiante y en espanol, por que los roles
     se siembran al inicio y como se usaria [Authorize(Roles = "Admin")].

NO proteger todavia controllers con [Authorize(Roles = ...)] salvo que ya
exista un controller de mantenimiento; eso se hace en el prompt del CRUD.

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, arquitectura N-capas.
```

---

## PROMPT 3 — Conexion de capas + CRUD de Retos (mantenimiento admin)

```
TAREA: Implementar la pantalla de mantenimiento de Retos (CRUD para el rol
Admin) conectando TODAS las capas: MVC -> Core -> Repositories -> Data.
Este es el modulo que demuestra el punto "Conexion completa de capas" del
Avance 2.

PASO 1 — ANALISIS PREVIO:
Lee y valida la cadena ya existente:
  AP.Data/Entities/Reto.cs y ModoJuego (enum)
  AP.Repositories/RepositoryBase.cs y RepositoryReto.cs
  AP.Core/Business/RetoBusiness.cs (reglas de dificultad 1-5 y tiempo > 0)
  AP.Core/Exceptions/AppException.cs
  AP.Models/Juegos/RetoViewModel.cs
Confirma que esa cadena esta completa y coherente. Lo UNICO que falta es la
capa MVC. Reporta cualquier hueco antes de continuar.

PASO 2 — IMPLEMENTACION:
  a) Crear un BaseController en AP.MVC/Controllers del que hereden los
     controllers del dominio. Debe capturar AppException y mostrarla como
     mensaje de advertencia (TempData) distinto de un error inesperado.
     Comentar por que se hace esta distincion (ya esta insinuado en el
     comentario de AppException.cs).
  b) Crear RetoController con acciones: Index (lista), Create (GET/POST),
     Edit (GET/POST), Delete (POST con confirmacion). Debe:
     - Usar RetoBusiness, NUNCA RepositoryReto ni MathemaXContext directo.
     - Mapear manualmente Reto <-> RetoViewModel con metodos privados
       (sin AutoMapper; somos estudiantes, mapeo explicito y claro).
     - Llenar CreatedBy/ModifiedBy con User.Identity.Name.
     - Protegerse con [Authorize(Roles = "Admin")] a nivel de clase.
  c) Crear las vistas Razor en Views/Reto/: Index.cshtml (tabla con las
     columnas principales, botones Editar/Eliminar, boton Nuevo Reto),
     Create.cshtml y Edit.cshtml (formulario compartido via parcial
     _FormReto.cshtml con validacion unobtrusive de MVC).
     - Usar el _Layout existente, clases Bootstrap 5 y la paleta de
       DISENO.md (clases mx-* si ya existen en Site.css).
     - Dropdown de Modo de juego a partir del enum ModoJuego.
     - Sin emojis, textos en espanol.
  d) Registrar el enlace del modulo en el sidebar de _Layout.cshtml,
     visible solo para el rol Admin (User.IsInRole("Admin")).

PASO 3 — VERIFICACION:
Lista los pasos manuales para que yo pruebe en Visual Studio: compilar,
loguearme como admin, crear/editar/eliminar un reto, verificar que la regla
de dificultad fuera de rango muestra el mensaje de AppException y no una
pantalla amarilla.

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, comentarios de
estudiante en espanol explicando el flujo entre capas.
```

---

## PROMPT 4 — Landing page / Home

```
TAREA: Reemplazar la Home de plantilla de ASP.NET por la landing page real de
MathemaX con la descripcion del juego (punto "Landing page / Home" del
Avance 2).

PASO 1 — ANALISIS PREVIO:
Lee AP.MVC/Views/Home/Index.cshtml (hoy tiene el contenido de ejemplo de
ASP.NET que hay que eliminar), About.cshtml, Contact.cshtml,
HomeController.cs, _Layout.cshtml y Content/Site.css (para reutilizar las
clases mx-* y la paleta de DISENO.md). Reporta que se conserva y que se
elimina antes de continuar.

PASO 2 — IMPLEMENTACION:
  a) Index.cshtml nuevo con:
     - Hero: nombre MathemaX, tagline de entrenamiento mental, boton
       "Comenzar a jugar" (lleva a Login si es anonimo, o a la futura
       pantalla de juego si esta autenticado; por ahora puede apuntar a
       una ruta placeholder comentada).
     - Seccion "Modos de juego": tres tarjetas con los modos del enum
       ModoJuego (Operador Perdido, Contrarreloj, Secuencias Logicas) con
       una descripcion corta de cada uno, usando Bootstrap Icons que ya
       estan cargados en el layout (iconos, NO emojis).
     - Seccion breve de como funciona el XP y los marcadores.
  b) Decidir y proponerme: eliminar About/Contact o convertirlos en una
     pagina "Acerca del equipo". No los dejes con texto de plantilla.
  c) Todo el CSS nuevo va en Site.css con prefijo mx-, usando las variables
     de color de DISENO.md. Tipografias: Playfair Display para titulos
     grandes, Plus Jakarta Sans para texto (ya importadas en _Layout).
  d) Contenido 100% en espanol, tono sobrio de proyecto academico.

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, vista responsive
(el layout ya maneja sidebar colapsable, no romper ese comportamiento).
```

---

## PROMPT 5 — Modulo Foro (estructura inicial)

```
TAREA: Crear la estructura inicial (esqueleto) del modulo Foro, segun el
punto "Modulo Foro (estructura inicial)" del Avance 2. En el Avance 3 se le
conectara la logica completa; aqui se deja la estructura en todas las capas
con datos de ejemplo.

PASO 1 — ANALISIS PREVIO:
Lee MathemaXContext.cs: veras que ya hay un comentario "Pendientes" que
menciona el DbSet<Publicacion> para el Foro. Lee tambien como estan hechos
Reto/RepositoryReto/RetoBusiness/RetoViewModel/RetoController para replicar
EXACTAMENTE el mismo patron por capas. Reporta el plan de archivos nuevos
antes de crear nada.

PASO 2 — IMPLEMENTACION (mismo patron que Reto en cada capa):
  a) AP.Data: entidad Publicacion (PublicacionId, UsuarioId, Titulo,
     Contenido, FechaPublicacion, Activo + campos de auditoria como Reto).
     Registrar DbSet<Publicacion> en MathemaXContext (quitar esa linea del
     comentario de pendientes). NO generar la migracion; indicame el comando
     Add-Migration exacto para que yo lo corra en Visual Studio.
     Agregar la tabla al script _db/MathemaX_Init.sql con el estilo
     IF NOT EXISTS del resto del script.
  b) AP.Repositories: IRepositoryPublicacion + RepositoryPublicacion
     heredando de RepositoryBase<Publicacion>, con una consulta
     GetActivasRecientes().
  c) AP.Core: ForoBusiness con constructor para inyeccion (igual que
     RetoBusiness) y una regla de negocio minima comentada (por ejemplo,
     titulo y contenido no vacios via AppException).
  d) AP.Models: PublicacionViewModel con DataAnnotations en espanol.
  e) AP.MVC: ForoController ([Authorize], hereda de BaseController) con
     Index y Create funcionales contra la BD; vistas Index.cshtml (lista de
     publicaciones estilo tarjetas) y Create.cshtml. Detalle/edicion/borrado
     quedan como TODO comentado para el Avance 3.
  f) Habilitar el enlace "Comunidad" del sidebar en _Layout.cshtml
     (hoy esta con clase disabled) apuntando a Foro/Index.

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, comentarios de
estudiante, respetar el flujo MVC -> Core -> Repositories -> Data.
```

---

## PROMPT 6 — Modulo Solicitud TI (Request IT)

```
TAREA: Crear la estructura inicial del modulo Solicitud TI (Request IT),
punto "Modulo Solicitud TI" del Avance 2. Mismo enfoque esqueleto que el
Foro: estructura completa por capas, logica minima, se completa en Avance 3.

PASO 1 — ANALISIS PREVIO:
Lee el patron por capas de Reto (y de Foro si ya existe en esta rama) y
AP.Services/ChatbotService.cs, que ya existe como servicio auxiliar pensado
para este modulo. Reporta el plan de archivos antes de crear nada.

PASO 2 — IMPLEMENTACION (mismo patron por capas):
  a) AP.Data: entidad SolicitudTI (SolicitudTIId, UsuarioId, Asunto,
     Descripcion, Estado como enum EstadoSolicitud {Abierta=1, EnProceso=2,
     Cerrada=3}, FechaCreacion + auditoria). DbSet en MathemaXContext
     (actualizar el comentario de pendientes). Tabla en _db/MathemaX_Init.sql
     estilo IF NOT EXISTS. Indicarme el comando Add-Migration, no ejecutarlo.
  b) AP.Repositories: IRepositorySolicitudTI + RepositorySolicitudTI con
     GetPorUsuario(string usuarioId) y GetPorEstado(EstadoSolicitud estado).
  c) AP.Core: SolicitudTIBusiness con regla minima (asunto y descripcion
     obligatorios via AppException) y comentario de estudiante explicando
     que el flujo de estados se implementa en el Avance 3.
  d) AP.Models: SolicitudTIViewModel con DataAnnotations en espanol.
  e) AP.MVC: SolicitudTIController ([Authorize], hereda de BaseController):
     - Index: el usuario ve SUS solicitudes; Admin y Support ven todas.
     - Create: formulario funcional que guarda en BD.
     - En la vista Create, integrar un panel simple de "asistente" que llama
       a ChatbotService.GetRespuesta() para mostrar la respuesta generica
       (dejar comentado que en Avance 3 sera interactivo).
  f) Habilitar el enlace "Soporte TI" del sidebar apuntando a
     SolicitudTI/Index.

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, comentarios de
estudiante, sin sobre-disenar.
```

---

## PROMPT 7 — Modulo Buzon de Quejas

```
TAREA: Crear la estructura inicial del modulo Buzon de Quejas, punto
"Modulo Buzon de Quejas" del Avance 2. Mismo enfoque esqueleto por capas.

PASO 1 — ANALISIS PREVIO:
Lee el patron por capas de los modulos existentes (Reto, y Foro/SolicitudTI
si ya estan en esta rama) para replicarlo. Verifica en MathemaXContext.cs el
comentario de pendientes donde figura DbSet<Queja>. Reporta el plan de
archivos antes de crear nada.

PASO 2 — IMPLEMENTACION (mismo patron por capas):
  a) AP.Data: entidad Queja (QuejaId, UsuarioId opcional para permitir queja
     anonima, Categoria como enum CategoriaQueja {Juego=1, Cuenta=2,
     Contenido=3, Otro=4}, Detalle, FechaCreacion, Atendida bool +
     auditoria). DbSet en MathemaXContext. Tabla en _db/MathemaX_Init.sql.
     Indicarme el comando Add-Migration, no ejecutarlo.
  b) AP.Repositories: IRepositoryQueja + RepositoryQueja con
     GetNoAtendidas().
  c) AP.Core: QuejaBusiness con regla minima (detalle obligatorio y longitud
     razonable via AppException).
  d) AP.Models: QuejaViewModel con DataAnnotations en espanol.
  e) AP.MVC: QuejaController (hereda de BaseController):
     - Create: accesible para usuarios autenticados; si se marca como
       anonima, no se guarda el UsuarioId (comentar la decision).
     - Index: SOLO Admin y Support ([Authorize(Roles = "Admin,Support")]),
       lista de quejas con filtro simple por atendidas/no atendidas.
     - Vista de confirmacion despues de enviar la queja (mensaje sobrio,
       sin emojis).
  f) Habilitar el enlace "Quejas" del sidebar apuntando a Queja/Create.

Recuerda: rama dev/sajmzrs, sin commits, sin emojis, comentarios de
estudiante, respetar arquitectura.
```

---

## PROMPT 8 — Comentarios SOLID, navegacion y cierre del Avance 2

```
TAREA: Cierre del Avance 2: identificar y comentar los principios SOLID en el
codigo, revisar la navegacion completa y actualizar la BITACORA.

PASO 1 — ANALISIS PREVIO:
Recorre todo el codigo del equipo (sin librerias de terceros) y detecta donde
YA se aplican principios SOLID de forma natural. Ejemplos que deberias
encontrar: SRP en la separacion por capas y en GlobalValidation; OCP/LSP en
RepositoryBase<T> y sus herederos; ISP en interfaces pequenas como
IRepositoryReto; DIP en RetoBusiness recibiendo IRepositoryReto por
constructor. Presenta la lista de ubicaciones ANTES de escribir comentarios.

PASO 2 — IMPLEMENTACION:
  a) Agregar comentarios SOLID en los puntos identificados usando EXACTAMENTE
     el formato que pide la BITACORA del Avance 3:
       // SOLID: [principio aplicado] - explicacion corta en espanol
     Un comentario por ubicacion real; no inventar aplicaciones forzadas ni
     comentar el mismo principio diez veces. Estilo estudiante: explicar en
     una linea por que ese codigo cumple el principio.
  b) Revisar el sidebar de _Layout.cshtml: todos los modulos del Avance 2
     (Home, Retos admin, Foro/Comunidad, Soporte TI, Quejas) deben tener su
     enlace funcionando y con la clase active correcta; "Marcadores" queda
     disabled con un comentario TODO Avance 3.
  c) Verificacion final de reglas del proyecto:
     - Ningun controller usa MathemaXContext o repositorios directamente.
     - Ninguna vista usa entidades de AP.Data (solo ViewModels).
     - Sin emojis en ningun archivo del equipo.
     - Sin usings muertos ni codigo comentado sobrante en lo nuevo.
     Reporta cualquier violacion encontrada y corrigela.
  d) Actualizar BITACORA.md: marcar con [x] los puntos del Avance 2 que
     quedaron completos, agregar la fecha real, y dejar en [ ] lo que
     honestamente quedo pendiente. No tocar las secciones de otros avances.

PASO 3 — ENTREGA:
Resumen final: archivos tocados, checklist de pruebas manuales en Visual
Studio, y recordatorio del flujo Git que sigue (yo lo ejecuto manualmente):
  1. Revisar diff completo en la rama dev/sajmzrs.
  2. Commits pequenos siguiendo _documentos/FLUJO_GIT.md (verbos en espanol).
  3. Push a origin dev/sajmzrs y PR hacia staging con review de otro companero.
  4. Probado staging en equipo -> PR staging a main.
  5. Solo en la entrega: merge main a production y tag avance-2.

Recuerda: rama dev/sajmzrs, sin commits automaticos, sin emojis.
```

---

## Notas finales

- Los prompts 5, 6 y 7 son independientes entre si, pero los tres dependen
  del BaseController y de los roles creados en los prompts 2 y 3. Ejecutar
  primero 1-4 y luego el orden de 5-7 es indiferente.
- Cada modulo nuevo requiere correr `Add-Migration` y `Update-Database` en
  Visual Studio manualmente; la IA solo deja indicado el comando.
- Si un prompt se corta o la sesion se pierde, volver a pegar el BLOQUE DE
  CONTEXTO completo antes de continuar.
