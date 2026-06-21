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

Abrir SQL Server Management Studio, conectarse a `localhost` y ejecutar:
```
_db/MathemaX_Init.sql
```

Esto crea la base de datos `MathemaX` con todas las tablas e inserta datos de ejemplo.

**3. Verificar cadena de conexión**

En `AP.MVC/Web.config` ajustar `Data Source` según la instancia local:
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

**5. Ejecutar migraciones de EF** (si es la primera vez)

En la Consola de Administrador de Paquetes (Tools → NuGet Package Manager):
```
PM> Update-Database
```

**6. Correr el proyecto**

Presionar `F5` o el botón de inicio en Visual Studio.

---

Para el flujo de trabajo con Git ver: [`_documentos/FLUJO_GIT.md`](_documentos/FLUJO_GIT.md)

Para el estado del proyecto ver: [`BITACORA.md`](BITACORA.md)
