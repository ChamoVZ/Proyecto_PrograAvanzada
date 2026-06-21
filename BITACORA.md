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

### Pendiente
- [ ] Convenciones de nomenclatura aplicadas en todo el código
- [ ] Conexión completa de capas: MVC → Core → Repositories → Data
- [ ] Vistas esqueleto para todos los módulos:
  - [ ] Landing page / Home con descripción del juego
  - [ ] Pantalla de mantenimiento de Retos (CRUD admin)
  - [ ] Módulo Foro (estructura inicial)
  - [ ] Módulo Solicitud TI (Request IT)
  - [ ] Módulo Buzón de Quejas
- [ ] Roles de Identity configurados: Admin, Player, Support
- [ ] Principios SOLID identificados y comentados en el código

---

## Avance 3 — Pantallas completas con lógica
**Fecha estimada:** Por confirmar | **Valor:** 20 pts

### Pendiente
- [ ] Todas las pantallas funcionales con lógica conectada
- [ ] Juego de Math Riddles jugable (al menos un modo)
- [ ] Sistema de XP y marcadores (tabla Partidas)
- [ ] Foro funcional (CRUD de publicaciones)
- [ ] Request IT funcional
- [ ] Buzón de Quejas funcional
- [ ] Comentarios SOLID y Design Patterns en código
  - Formato: `// SOLID: [principio aplicado]`
  - Formato: `// DP: [patrón aplicado]`

---

## Avance 4 — Trabajo completo
**Fecha estimada:** Semana 14, 2026 | **Valor:** 20 pts

### Pendiente
- [ ] Todos los módulos terminados y conectados
- [ ] Pruebas unitarias completas en AP.Tests
- [ ] Sin código muerto ni vistas vacías
- [ ] README actualizado con instrucciones de setup
- [ ] Tag `avance-4` creado en rama `production`

---

## Demo
**Fecha:** Por confirmar | **Valor:** 20 pts

### Pendiente
- [ ] Preparar flujo de demostración
- [ ] Verificar que compila en una máquina limpia
- [ ] Tener datos de prueba listos en la BD
