using System.Configuration;
using AP.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AP.MVC.Startup))]
namespace AP.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            CreateUsers();
        }

        private void CreateRoles()
        {
            // Instanciamos el contexto y el manejador de roles
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Creamos los roles si no existen (idempotente)
            // Esto permite usar [Authorize(Roles = "Admin")] en los controladores para restringir el acceso.
            string[] roleNames = { "Admin", "Player", "Support" };

            foreach (var roleName in roleNames)
            {
                if (!roleManager.RoleExists(roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    roleManager.Create(role);
                }
            }
        }

        // Crea un usuario de prueba por cada rol si no existen (idempotente).
        // Las claves salen del Web.config local, que no se versiona. Solo para uso local.
        private void CreateUsers()
        {
            var context = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            SeedUser(userManager, "admin@mathemax.local", "Seed:AdminPassword", "Admin");
            SeedUser(userManager, "player@mathemax.local", "Seed:PlayerPassword", "Player");
            SeedUser(userManager, "support@mathemax.local", "Seed:SupportPassword", "Support");
        }

        private void SeedUser(ApplicationUserManager userManager, string email, string claveAppSetting, string rol)
        {
            // Sin la clave configurada el usuario no se siembra, para no dejar credenciales en el repositorio.
            var password = ConfigurationManager.AppSettings[claveAppSetting];
            if (string.IsNullOrWhiteSpace(password))
                return;

            if (userManager.FindByName(email) != null)
                return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var resultado = userManager.Create(user, password);
            if (resultado.Succeeded)
            {
                userManager.AddToRole(user.Id, rol);
            }
        }
    }
}
