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
            catch (System.Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al publicar. Intente más tarde.";
                return View(model);
            }
        }

        // GET: Foro/Details/5
        public ActionResult Details(int id)
        {
            var publicacion = _foroBusiness.GetPorId(id);
            if (publicacion == null)
            {
                return HttpNotFound();
            }

            return View(MapToViewModel(publicacion));
        }

        // GET: Foro/Edit/5
        public ActionResult Edit(int id)
        {
            var publicacion = _foroBusiness.GetPorId(id);
            if (publicacion == null)
            {
                return HttpNotFound();
            }

            if (!PuedeModificar(publicacion))
            {
                TempData["ErrorMessage"] = "Solo el autor puede modificar esta publicación.";
                return RedirectToAction("Index");
            }

            return View(MapToViewModel(publicacion));
        }

        // POST: Foro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PublicacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var publicacion = _foroBusiness.GetPorId(model.PublicacionId);
            if (publicacion == null)
            {
                return HttpNotFound();
            }

            if (!PuedeModificar(publicacion))
            {
                TempData["ErrorMessage"] = "Solo el autor puede modificar esta publicación.";
                return RedirectToAction("Index");
            }

            try
            {
                // Recargamos la entidad y solo tocamos lo editable, así no perdemos UsuarioId ni la auditoría de creación.
                publicacion.Titulo = model.Titulo;
                publicacion.Contenido = model.Contenido;
                publicacion.ModifiedBy = User.Identity.Name;

                _foroBusiness.Actualizar(publicacion);
                return RedirectToAction("Index");
            }
            catch (AP.Core.Exceptions.AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        // POST: Foro/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var publicacion = _foroBusiness.GetPorId(id);
            if (publicacion == null)
            {
                return HttpNotFound();
            }

            if (!PuedeModificar(publicacion))
            {
                TempData["ErrorMessage"] = "Solo el autor puede eliminar esta publicación.";
                return RedirectToAction("Index");
            }

            _foroBusiness.Desactivar(id);
            return RedirectToAction("Index");
        }

        // El autor gestiona su publicación; el Admin puede moderar cualquiera.
        private bool PuedeModificar(Publicacion publicacion)
        {
            return publicacion.UsuarioId == User.Identity.GetUserId() || User.IsInRole("Admin");
        }

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
