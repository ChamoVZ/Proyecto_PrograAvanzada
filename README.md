# MathemaX — Math Riddles

Plataforma web de agilidad mental y ejercicios matemáticos. Proyecto final del curso SC-601 Programación Avanzada.

## Equipo

| GitHub | Rama de trabajo |
|--------|-----------------|
| @ChamoVZ | `dev/ChamoVZ` |
| @ManuelASV | `dev/ManuelASV` |
| @Evan | `dev/Evan` |
| @sajmzrs | `dev/sajmzrs` |

## Stack tecnológico

- ASP.NET MVC 5 / .NET Framework 4.8
- Entity Framework 6 (Code First)
- ASP.NET Identity 2
- SQL Server Express (localhost)
- Bootstrap 5 + Bootstrap Icons
- jQuery 3.7

## Estructura del proyecto

```
AP.sln
├── AP.Data          → entidades EF, DbContext, migraciones
├── AP.Repositories  → patrón repositorio (genérico + específicos)
├── AP.Core          → lógica de negocio, excepciones, validaciones
├── AP.Services      → servicios auxiliares (email, chatbot)
├── AP.Models        → ViewModels y DTOs para las vistas
├── AP.MVC           → controllers, vistas Razor, Identity, configuración
└── AP.Tests         → pruebas unitarias con MSTest
```

Las dependencias fluyen hacia abajo: `MVC → Core/Models → Repositories → Data`. Ninguna capa inferior referencia a una superior.

## Setup local

### Prerequisitos
- Visual Studio 2022
- SQL Server Express (instancia `localhost` o `.\SQLEXPRESS`)
- .NET Framework 4.8 SDK

### Pasos

**1. Clonar el repositorio**
```bash
git clone https://github.com/ChamoVZ/Proyecto_PrograAvanzada.git
cd Proyecto_PrograAvanzada
git checkout dev/tu-rama
```

**2. Restaurar base de datos**

Abrir SQL Server Management Studio, conectarse a `localhost` y ejecutar los scripts **en este orden**:

```
1) _db/MathemaX_Init.sql                        → crea la base MathemaX con las tablas base y datos de ejemplo
2) _db/scripts/2026-07-12_experiencia_y_retos.sql → agrega columnas de experiencia (ExperienciaTotal, Nivel) y siembra retos de los modos de juego
3) _db/scripts/2026-07-27_desactivar_retos_duplicados.sql → desactiva retos duplicados de un seed viejo (en una base nueva no hace nada)
```

El segundo script valida que el primero ya se haya corrido, así que no se puede ejecutar solo. Las columnas de experiencia se agregan por script porque viven en la tabla de Identity (`AspNetUsers`), que no usa el inicializador de EF.

Estos scripts son la única fuente del esquema: **no** hay que correr `Update-Database` ni ninguna migración de EF (los inicializadores están desactivados en `Global.asax.cs`).

**3. Configurar la cadena de conexión**

El `Web.config` real no se versiona (cada quien usa su propia instancia de SQL Server). Copiar la plantilla y ajustar el `Data Source`:

```bash
cp AP.MVC/Web.config.example AP.MVC/Web.config
```

En el `Web.config` recién copiado, ajustar `Data Source` según la instancia local:
```xml
<add name="MathemaXContext"
     connectionString="Data Source=localhost;Initial Catalog=MathemaX;Integrated Security=True;TrustServerCertificate=True"
     providerName="System.Data.SqlClient" />
```

Si SQL Server usa instancia con nombre: `Data Source=.\SQLEXPRESS`

**4. Compilar en Visual Studio**
- Abrir `AP.sln`
- Click derecho en la solución → **Restore NuGet Packages**
- **Build → Build Solution** (`Ctrl+Shift+B`)

**5. Correr el proyecto**

Presionar `F5` o el botón de inicio en Visual Studio.
