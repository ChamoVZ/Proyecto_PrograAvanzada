# Manual de Trabajo con Git — MathemaX

## Estructura de ramas

| Rama | Propósito |
|------|-----------|
| `main` | Código estable y revisado. Solo recibe merges desde `staging`. |
| `staging` | Rama de integración y pruebas del equipo. Los merges de dev/* llegan aquí primero. |
| `production` | Entregables finales al profesor. Se crea una release aquí antes de cada avance. |
| `dev/sajmzrs` | Rama personal de Sebastian |
| `dev/ChamoVZ` | Rama personal de Chamo |
| `dev/ManuelASV` | Rama personal de Manuel |
| `dev/Evan` | Rama personal de Evan |

**Flujo de trabajo:**

```
dev/nombre  →  staging  →  main  →  production (solo en entregas)
```

- Nadie hace commit directo a `main` ni a `production`.
- Los merges a `staging` se hacen mediante Pull Request en GitHub.
- Los merges de `staging` a `main` también van por PR, una vez que el equipo verifica que todo compila y funciona.

---

## Flujo diario de trabajo

### Al comenzar a trabajar

```bash
# 1. Siempre posicionarse en la rama propia antes de cualquier cosa
git checkout dev/nombre

# 2. Traer los últimos cambios del repositorio remoto
git pull origin dev/nombre

# 3. Sincronizar con los cambios que ya pasaron a staging
git pull origin staging
```

Si hay conflictos al hacer el pull de staging, resolverlos antes de continuar (ver sección de conflictos).

### Durante el trabajo

Hacer commits pequeños y frecuentes, no acumular cambios grandes en un solo commit.

```bash
# Ver qué archivos cambiaron
git status

# Agregar solo los archivos relevantes (no usar git add . sin revisar antes)
git add AP.MVC/Controllers/RetoController.cs
git add AP.MVC/Views/Reto/

# Commit en español, imperativo, máximo 72 caracteres
git commit -m "Agregar controller y vistas de Reto"
```

### Al terminar la sesión de trabajo

```bash
# Subir los cambios de la rama propia al repositorio remoto
git push origin dev/nombre
```

---

## Convenciones de commit

### Formato

```
<acción> <descripción corta en español>
```

### Verbos permitidos

| Verbo | Cuándo usarlo |
|-------|---------------|
| `Agregar` | Código nuevo, archivos nuevos, funcionalidades |
| `Actualizar` | Modificar algo que ya existía |
| `Corregir` | Bug fix |
| `Eliminar` | Borrar código o archivos innecesarios |
| `Refactorizar` | Reestructurar sin cambiar comportamiento |
| `Configurar` | Cambios en archivos de configuración |

### Ejemplos correctos

```
Agregar controller y vista de Foro
Corregir validación de dificultad en RetoBusiness
Actualizar cadena de conexión para SQL Server Express
Refactorizar RepositoryBase para reutilizar GetAll
Configurar rutas en RouteConfig para módulo de quejas
```

### Ejemplos incorrectos

```
fix bug         ← en inglés y muy vago
cambios         ← no dice qué cambió
WIP             ← nunca subir trabajo sin terminar a staging
Agregar muchas cosas y corregir otras ← demasiado en un commit
```

---

## Cómo hacer un Pull Request (PR)

Un PR es la forma de pasar cambios de `dev/nombre` a `staging`.

1. Subir la rama al remoto: `git push origin dev/nombre`
2. Ir a GitHub → Pull Requests → New Pull Request
3. **Base:** `staging` | **Compare:** `dev/nombre`
4. Título del PR: igual que el commit principal (español, descriptivo)
5. Descripción: qué se agregó, qué se probó, si hay algo pendiente
6. Pedir review a al menos otro integrante antes de hacer merge
7. Nunca hacer merge del PR propio sin que otro lo revise

---

## Resolución de conflictos

Un conflicto ocurre cuando dos personas modificaron el mismo archivo en las mismas líneas.

```bash
# Después de un pull que genere conflicto, git indica los archivos afectados
git status

# Abrir el archivo en conflicto. Tendrá marcadores como:
# <<<<<<< HEAD (mis cambios)
# ...
# =======
# ...
# >>>>>>> staging (cambios del otro)

# Editar el archivo manualmente para quedarse con la versión correcta
# Eliminar todos los marcadores (<<<, ===, >>>)

# Marcar el conflicto como resuelto
git add nombre-del-archivo.cs

# Continuar con el commit
git commit -m "Resolver conflicto en NombreArchivo"
```

**Regla:** Nunca dejar los marcadores `<<<<<<<`, `=======`, `>>>>>>>` en el código. Si hay duda sobre qué versión conservar, consultarlo con el equipo antes de resolver.

---

## Reglas que no se deben romper

- **No hacer force push** (`git push --force`) a ninguna rama compartida (staging, main, production). Reescribe el historial y causa pérdida de trabajo de otros.
- **No hacer commit directamente a `main`** sin haber pasado por staging y PR.
- **No subir paquetes NuGet** (carpeta `packages/`). Ya están en `.gitignore`. Se restauran con `NuGet Restore` en Visual Studio.
- **No subir carpetas `bin/` ni `obj/`**. Se generan al compilar y no tienen lugar en el repo.
- **No subir archivos `.vs/`** ni `*.user`. Son configuraciones personales de Visual Studio.
- **No hacer merge de un PR propio** sin revisión de otro integrante.

---

## Restaurar el proyecto en una máquina nueva

```bash
# 1. Clonar el repo
git clone https://github.com/ChamoVZ/Proyecto_PrograAvanzada.git
cd Proyecto_PrograAvanzada

# 2. Posicionarse en la rama de trabajo
git checkout dev/nombre

# 3. Abrir AP.sln en Visual Studio 2022
#    → Click derecho en la solución → Restore NuGet Packages
#    → Build → Build Solution (Ctrl+Shift+B)

# 4. Restaurar la base de datos
#    → Abrir SQL Server Management Studio
#    → Conectar a localhost (o .\SQLEXPRESS)
#    → Abrir y ejecutar el archivo _db/MathemaX_Init.sql

# 5. Verificar la cadena de conexión en AP.MVC/Web.config
#    Si usa instancia con nombre: Data Source=.\SQLEXPRESS
#    Si usa instancia default: Data Source=localhost

# 6. Ejecutar las migraciones de EF desde la consola de NuGet en VS:
#    PM> Update-Database

# 7. Correr el proyecto con F5
```

---

## Cuándo usar cada rama — resumen rápido

| Situación | Acción |
|-----------|--------|
| Trabajando en una funcionalidad nueva | Hacer commits en `dev/nombre` |
| Terminar una funcionalidad y quiero que el equipo la use | PR de `dev/nombre` → `staging` |
| El equipo verificó que staging compila y funciona | PR de `staging` → `main` |
| Entrega de un avance al profesor | Merge de `main` → `production`, crear tag `avance-N` |
| Encontré un bug urgente en staging | Corregirlo en `dev/nombre`, PR a staging con descripción del fix |
