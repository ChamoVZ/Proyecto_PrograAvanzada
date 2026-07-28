# Bitácora — MathemaX
**Proyecto SC-601 Programación Avanzada**
**Equipo:** ChamoVZ · ManuelASV · Evan · sajmzrs

---

## Avance 1 — Estructura base del proyecto
**Fecha:** 2026-06-21 | **Valor:** 20 pts

### Completado
- [x] Repositorio del equipo creado en GitHub (`ChamoVZ/Proyecto_PrograAvanzada`)
- [x] Solución ASP.NET MVC 5 con arquitectura N-capas
  - AP.Data — entidades EF y DbContext
  - AP.Repositories — repositorio genérico y repositorio de Retos
  - AP.Core — lógica de negocio (RetoBusiness, AppException, GlobalValidation)
  - AP.Services — servicios auxiliares (EmailService, ChatbotService)
  - AP.Models — ViewModels y DTOs (RetoViewModel)
  - AP.MVC — controllers, vistas Razor, Identity
  - AP.Tests — pruebas unitarias con MSTest
- [x] Autenticación con ASP.NET Identity 2 (login/registro ya funcional)
- [x] Base de datos diseñada: tablas Retoes y Partidas + tablas de Identity
- [x] Script SQL de inicialización (`_db/MathemaX_Init.sql`)
- [x] Migración inicial de Entity Framework (`202606100557149_Inicial`)
- [x] Layout visual base — sidebar, paleta de colores, tipografías (DISENO.md)
- [x] `.gitignore` configurado (excluye bin, obj, packages, .vs)
- [x] Ramas del equipo creadas: `main`, `staging`, `production`, `dev/*`
- [x] Manual de flujo de trabajo Git (`_documentos/FLUJO_GIT.md`)

### Entregables del Avance 1
- [x] Repositorio GitHub con estructura del proyecto
- [x] Script SQL de la base de datos en `_db/MathemaX_Init.sql`

---

## Avance 2 — Lógica de negocio y vistas base
**Fecha estimada:** Por confirmar | **Valor:** 20 pts

### Estado (actualizado 2026-07-11)
- [ ] Convenciones de nomenclatura aplicadas en todo el código (falta revisión final contra `_documentos/CONVENCIONES.md`)
- [x] Conexión completa de capas: MVC → Core → Repositories → Data (funcionando en Retos, Foro, Solicitud TI y Quejas)
- [x] Vistas esqueleto para todos los módulos:
  - [x] Landing page / Home con descripción del juego
  - [x] Pantalla de mantenimiento de Retos (CRUD admin)
  - [x] Módulo Foro (estructura inicial)
  - [x] Módulo Solicitud TI (Request IT)
  - [x] Módulo Buzón de Quejas
- [x] Roles de Identity configurados: Admin, Player, Support (seed en `Startup.CreateRoles()`, rol Player al registrarse, `[Authorize(Roles)]` en controladores)
- [x] Principios SOLID identificados y comentados en el código (completado en el Avance 3)

### Notas
- 2026-07-11: se corrigió staging agregando la migración `InicializarQueja`, que faltó al subir el módulo Buzón de Quejas (el modelo EF quedó desincronizado de `__MigrationHistory` y daba error al correr la app en BDs creadas con migraciones). Acordado: toda entidad nueva debe subir con su migración.

---

## Avance 3 — Pantallas completas con lógica
**Fecha:** 2026-07-22 | **Valor:** 20 pts

### Completado
- [x] Juego de Math Riddles jugable (modo Operador Perdido, de punta a punta)
- [x] Modo Contrarreloj jugable, con tiempo validado en servidor, bono de XP y resultado persistido
- [x] Sistema de XP y marcadores (XP persistido en `AspNetUsers`; ranking e historial desde la tabla `Partidas`)
- [x] Foro funcional (CRUD de publicaciones con borrado lógico y validación de autor/Admin)
- [x] Request IT funcional (crear, listar y cambio de estado por soporte)
- [x] Buzón de Quejas funcional (crear, listar y cambio de estado por soporte)
- [x] Comentarios SOLID y Design Patterns en código (Business, Repositories y Controllers)
  - Formato: `// SOLID: [principio aplicado]`
  - Formato: `// DP: [patrón aplicado]`

### Pendiente / parcial
- [ ] Secuencias Lógicas: la estrategia y el reto inicial ya existen, pero falta conectar el modo a un controlador y a sus vistas.
- [ ] Solicitud TI y Buzón de Quejas sin edición/borrado por el autor (por ahora solo creación y cambio de estado).

### Notas
- Base del avance (Evan, 2026-07-11/12): funcionalidad de experiencia y modo Operador Perdido. Correcciones del juego y persistencia de XP con `UserManager` (Sebastián, 2026-07-12).
- Cierre del avance (Sebastián, 2026-07-22): patrón Strategy en `AP.Core` alineado con el PDF de diseño, marcadores, CRUD del Foro, cambio de estado en Solicitud TI/Quejas, comentarios SOLID/DP y correcciones menores.
- Contrarreloj (Sebastián, 2026-07-25): controlador y vistas conectados a la estrategia existente, validación de reto activo y modo, y transacción compartida para registrar la partida y actualizar XP de forma atómica.
- Excepción al acuerdo del Avance 2: `ExperienciaTotal`/`Nivel` se agregaron por script SQL (`_db/scripts/2026-07-12_experiencia_y_retos.sql`), no por migración EF, por ser columnas de `AspNetUsers` (Identity, sin inicializador de EF). El script debe correrse tras `MathemaX_Init.sql`.
- Los scripts SQL incluyen retos iniciales para Operador Perdido, Contrarreloj y Secuencias Lógicas.
- Validación del 2026-07-25: solución y vistas Razor compilaron sin errores; las 36 pruebas disponibles finalizaron correctamente después del ajuste transaccional.

---

## Avance 4 — Trabajo completo
**Fecha estimada:** Semana 14, 2026 | **Valor:** 20 pts

### Completado
- [x] Codificación UTF-8 unificada en todo el proyecto (ver `_documentos/CONVENCIONES.md`)
- [x] Selector de modos de juego: la opción "Jugar" del menú abre `/Juego/Index` en vez de entrar directo a Operador Perdido

### Pendiente
- [ ] Todos los módulos terminados y conectados
- [ ] Ampliar AP.Tests a los módulos que todavía no tienen cobertura
- [ ] Sin código muerto ni vistas vacías
- [ ] README actualizado con instrucciones de setup
- [ ] Tag `avance-4` creado en rama `production`

---

### Notas
- Codificación (Sebastián, 2026-07-27): no había texto dañado en el repositorio, pero sí la causa. Faltaba `<globalization>` en el `Web.config` y varias vistas con tildes estaban guardadas en UTF-8 sin BOM, entre ellas `_Layout.cshtml`; sin esas dos cosas ASP.NET las lee con el codepage ANSI de Windows. Para esquivarlo el equipo había usado tres estilos distintos a la vez —entidades HTML, escapes `\uXXXX` y tildes literales—, a veces para el mismo mensaje en archivos diferentes. Se corrigió la causa y se unificó todo a tildes literales sobre UTF-8 con BOM.
- De paso se detectó que `_db/scripts/2026-07-12_experiencia_y_retos.sql` sembraba los mismos retos con los títulos sin tilde, y como deduplica comparando por `Titulo` insertaba duplicados de los de `MathemaX_Init.sql`. Ya coinciden los dos scripts. En las BD donde ya se corrieron ambos quedaron retos repetidos: se limpian con `_db/scripts/2026-07-27_desactivar_retos_duplicados.sql`, que los desactiva en vez de borrarlos porque algunos ya tienen partidas jugadas.
- Selector de modos (Sebastián, 2026-07-27): `JuegoController` + `Views/Juego/Index.cshtml` con las tarjetas de los tres modos. Secuencias Lógicas aparece marcado como "Próximamente" porque sigue sin controlador. El resaltado del menú ahora se enciende con cualquier controlador de juego, no solo con Operador Perdido.

---

## Demo
**Fecha:** Por confirmar | **Valor:** 20 pts

### Pendiente
- [ ] Preparar flujo de demostración
- [ ] Verificar que compila en una máquina limpia
- [ ] Tener datos de prueba listos en la BD
