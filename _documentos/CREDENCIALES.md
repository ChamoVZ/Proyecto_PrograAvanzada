# Credenciales de prueba — MathemaX

Usuarios sembrados automáticamente al iniciar la aplicación (ver `Startup.CreateUsers()`).
Se crean solo si no existen, así que basta con correr la app una vez tras aplicar los scripts de BD.


| Rol | Usuario (correo) | Contraseña |
|-----|------------------|------------|
| Admin | `admin@mathemax.local` | `Admin123!` |
| Player | `player@mathemax.local` | `Player123!` |
| Support | `support@mathemax.local` | `Support123!` |

## Notas

- El inicio de sesión es con el **correo** como nombre de usuario.
- Los tres roles (`Admin`, `Player`, `Support`) se siembran en `Startup.CreateRoles()`.
- Requisito de contraseña (configurado en `IdentityConfig.cs`): mínimo 6 caracteres, con mayúscula, minúscula, dígito y un símbolo.
- Si los usuarios no aparecen, verificar que se corrieron los scripts de BD en orden (`_db/MathemaX_Init.sql` y luego `_db/scripts/2026-07-12_experiencia_y_retos.sql`), ya que la tabla `AspNetUsers` necesita las columnas `ExperienciaTotal` y `Nivel`.
