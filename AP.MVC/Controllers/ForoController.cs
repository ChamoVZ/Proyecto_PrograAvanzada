using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Data.Entities;
using AP.Models.Comunidad;
using Microsoft.AspNet.Identity;

namespace AP.MVC.Controllers
{
    // Solo usuarios autenticados pueden ver y crear en el foro
    [Authorize]
    public class ForoController : BaseController
    {
        private readonly ForoBusiness _foroBusiness;
        // Para inyectar el repositorio manualmente como en Reto
        public ForoController()
        {
            _foroBusiness = new ForoBusiness();
        }

        // GET: Foro
        public ActionResult Index()
        {
            var publicaciones = _foroBusiness.GetActivasRecientes();
            var viewModels = publicaciones.Select(MapToViewModel).ToList();
            return View(viewModels);
        }

        // GET: Foro/Create
        public ActionResult Create()
        {
            return View(new PublicacionViewModel { Activo = true });
        }

        // POST: Foro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PublicacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var publicacion = MapToEntity(model);
                
                // Asignar el ID del usuario logueado usando Identity
                publicacion.UsuarioId = User.Identity.GetUserId();
                publicacion.CreatedBy = User.Identity.Name;

                // Save aplica reglas de negocio y arroja AppException si hay error
                _foroBusiness.Save(publicacion);
                
                return RedirectToAction("Index");
            }
            catch (AP.Core.Exceptions.AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al publicar. Intente más tarde.";
                return View(model);
            }
        }

        // TODO: Avance 3 - GET/POST Edit
        // TODO: Avance 3 - GET/POST Delete
        // TODO: Avance 3 - GET Details

        #region Mapeo Manual

        private PublicacionViewModel MapToViewModel(Publicacion entity)
        {
            return new PublicacionViewModel
            {
                PublicacionId = entity.PublicacionId,
                UsuarioId = entity.UsuarioId,
                Titulo = entity.Titulo,
                Contenido = entity.Contenido,
                FechaPublicacion = entity.FechaPublicacion,
                Activo = entity.Activo,
                // El nombre del autor se podría cruzar con la tabla AspNetUsers en la capa Business o Repositorio en el futuro,
                // por ahora usaremos CreatedBy como workaround simple (contiene el email/username si se asignó al crear)
                NombreAutor = entity.CreatedBy ?? "Usuario"
            };
        }

        private Publicacion MapToEntity(PublicacionViewModel model)
        {
            return new Publicacion
            {
                PublicacionId = model.PublicacionId,
                UsuarioId = model.UsuarioId,
                Titulo = model.Titulo,
                Contenido = model.Contenido,
                FechaPublicacion = model.FechaPublicacion,
                Activo = model.Activo
            };
        }

        #endregion
    }
}
