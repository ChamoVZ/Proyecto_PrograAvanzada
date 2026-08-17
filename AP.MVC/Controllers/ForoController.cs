using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Data;
using AP.Data.Entities;
using AP.Models.Comunidad;
using Microsoft.AspNet.Identity;

namespace AP.MVC.Controllers
{
    // Solo usuarios autenticados pueden ver y crear en el foro
    [Authorize]
    public class ForoController : BaseController
    {
        // El contexto es del controller: se abre uno solo por request y se libera al final.
        private readonly MathemaXContext _context;
        private readonly ForoBusiness _foroBusiness;

        public ForoController()
        {
            _context = new MathemaXContext();
            _foroBusiness = new ForoBusiness(_context);
        }

        // GET: Foro
        public ActionResult Index(int pagina = 1, int tamanoPagina = 10)
        {
            var publicaciones = _foroBusiness.GetActivasRecientes(pagina, tamanoPagina);
            var usuarioId = User.Identity.GetUserId();
            var esAdmin = User.IsInRole("Admin");
            var viewModels = new AP.Models.ResultadoPaginado<PublicacionViewModel>
            {
                Elementos = publicaciones.Elementos
                    .Select(p => MapToViewModel(p, usuarioId, esAdmin))
                    .ToList(),
                PaginaActual = publicaciones.PaginaActual,
                TamanoPagina = publicaciones.TamanoPagina,
                TotalRegistros = publicaciones.TotalRegistros,
                TotalPaginas = publicaciones.TotalPaginas
            };

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

            // Una publicación desactivada no debe seguir siendo visible por URL directa, salvo para moderación.
            if (!publicacion.Activo && !User.IsInRole("Admin"))
            {
                return HttpNotFound();
            }

            return View(MapToViewModel(publicacion, User.Identity.GetUserId(), User.IsInRole("Admin")));
        }

        // GET: Foro/Edit/5
        public ActionResult Edit(int id)
        {
            var publicacion = _foroBusiness.GetPorId(id);
            if (publicacion == null)
            {
                return HttpNotFound();
            }

            var usuarioId = User.Identity.GetUserId();
            var esAdmin = User.IsInRole("Admin");
            if (!_foroBusiness.PuedeModificar(publicacion, usuarioId, esAdmin))
            {
                TempData["ErrorMessage"] = "Solo el autor o un administrador pueden modificar esta publicación.";
                return RedirectToAction("Index");
            }

            return View(MapToViewModel(publicacion, usuarioId, esAdmin));
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

            try
            {
                // Recargamos la entidad y solo tocamos lo editable, así no perdemos UsuarioId ni la auditoría de creación.
                publicacion.Titulo = model.Titulo;
                publicacion.Contenido = model.Contenido;
                publicacion.ModifiedBy = User.Identity.Name;

                _foroBusiness.Actualizar(publicacion, User.Identity.GetUserId(), User.IsInRole("Admin"));
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
        public ActionResult Delete(
            int id)
        {
            try
            {
                _foroBusiness.Desactivar(id, User.Identity.GetUserId(), User.IsInRole("Admin"));
            }
            catch (AP.Core.Exceptions.AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Mapeo Manual

        private PublicacionViewModel MapToViewModel(Publicacion entity, string usuarioId, bool esAdmin)
        {
            return new PublicacionViewModel
            {
                PuedeModificar = _foroBusiness.PuedeModificar(entity, usuarioId, esAdmin),
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
