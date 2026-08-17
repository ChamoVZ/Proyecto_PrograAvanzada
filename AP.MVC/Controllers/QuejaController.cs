using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data;
using AP.Data.Entities;
using AP.Models.Buzon;
using Microsoft.AspNet.Identity;

namespace AP.MVC.Controllers
{
    [Authorize]
    public class QuejaController : BaseController
    {
        // El contexto es del controller: se abre uno solo por request y se libera al final.
        private readonly MathemaXContext _context;
        private readonly QuejaBusiness _quejaBusiness;

        public QuejaController()
        {
            _context = new MathemaXContext();
            _quejaBusiness = new QuejaBusiness(_context);
        }

        // GET: Queja
        public ActionResult Index(int pagina = 1, int tamanoPagina = 10)
        {
            var usuarioId = User.Identity.GetUserId();

            // Admin y Support ven todas las quejas; el jugador solo ve las suyas.
            var quejas = User.IsInRole("Admin") || User.IsInRole("Support")
                ? _quejaBusiness.GetTodas(pagina, tamanoPagina)
                : _quejaBusiness.GetPorUsuario(usuarioId, pagina, tamanoPagina);

            var esAdmin = User.IsInRole("Admin");
            var viewModels = new AP.Models.ResultadoPaginado<QuejaViewModel>
            {
                Elementos = quejas.Elementos
                .Select(q => MapToViewModel(q, usuarioId, esAdmin))
                .ToList(),
                PaginaActual = quejas.PaginaActual,
                TamanoPagina = quejas.TamanoPagina,
                TotalRegistros = quejas.TotalRegistros,
                TotalPaginas = quejas.TotalPaginas
            };

            return View(viewModels);
        }

        // GET: Queja/Create
        public ActionResult Create()
        {
            return View(new QuejaViewModel
            {
                Categoria = (int)CategoriaQueja.Otro,
                Estado = (int)EstadoQueja.Pendiente
            });
        }

        // POST: Queja/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuejaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var queja = MapToEntity(model);
                queja.UsuarioId = User.Identity.GetUserId();
                queja.CreatedBy = User.Identity.Name;

                _quejaBusiness.Save(queja);

                return RedirectToAction("Index");
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
            catch (System.Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al registrar la queja. Intente más tarde.";
                return View(model);
            }
        }

        // GET: Queja/Edit/5
        public ActionResult Edit(int id)
        {
            var queja = _quejaBusiness.GetPorId(id);
            if (queja == null || !queja.Activo)
            {
                return HttpNotFound();
            }

            var usuarioId = User.Identity.GetUserId();
            if (!_quejaBusiness.PuedeEditar(queja, usuarioId))
            {
                TempData["ErrorMessage"] = "Solo el autor puede editar su queja, y solo mientras siga pendiente.";
                return RedirectToAction("Index");
            }

            return View(MapToViewModel(queja, usuarioId, User.IsInRole("Admin")));
        }

        // POST: Queja/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuejaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var queja = _quejaBusiness.GetPorId(model.QuejaId);
            if (queja == null || !queja.Activo)
            {
                return HttpNotFound();
            }

            try
            {
                // Recargamos la entidad y solo tocamos lo editable, asi no perdemos UsuarioId,
                // el estado que puso soporte ni la auditoria de creacion.
                queja.Asunto = model.Asunto;
                queja.Descripcion = model.Descripcion;
                queja.Categoria = (CategoriaQueja)model.Categoria;
                queja.ModifiedBy = User.Identity.Name;

                _quejaBusiness.Actualizar(queja, User.Identity.GetUserId());

                return RedirectToAction("Index");
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        // POST: Queja/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(
            int id)
        {
            try
            {
                _quejaBusiness.Desactivar(id, User.Identity.GetUserId(), User.IsInRole("Admin"));
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Queja/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Support")]
        public ActionResult CambiarEstado(int id, int estado)
        {
            try
            {
                _quejaBusiness.CambiarEstado(id, (EstadoQueja)estado);
            }
            catch (AppException ex)
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

        private QuejaViewModel MapToViewModel(Queja entity, string usuarioId, bool esAdmin)
        {
            return new QuejaViewModel
            {
                PuedeEditar = _quejaBusiness.PuedeEditar(entity, usuarioId),
                PuedeEliminar = _quejaBusiness.PuedeEliminar(entity, usuarioId, esAdmin),
                QuejaId = entity.QuejaId,
                UsuarioId = entity.UsuarioId,
                Asunto = entity.Asunto,
                Descripcion = entity.Descripcion,
                Categoria = (int)entity.Categoria,
                Estado = (int)entity.Estado,
                FechaCreacion = entity.FechaCreacion,
                NombreAutor = entity.CreatedBy ?? "Usuario",
                NombreCategoria = GetNombreCategoria(entity.Categoria),
                NombreEstado = GetNombreEstado(entity.Estado)
            };
        }

        private Queja MapToEntity(QuejaViewModel model)
        {
            return new Queja
            {
                QuejaId = model.QuejaId,
                UsuarioId = model.UsuarioId,
                Asunto = model.Asunto,
                Descripcion = model.Descripcion,
                Categoria = (CategoriaQueja)model.Categoria,
                Estado = (EstadoQueja)model.Estado,
                FechaCreacion = model.FechaCreacion
            };
        }

        private string GetNombreCategoria(CategoriaQueja categoria)
        {
            switch (categoria)
            {
                case CategoriaQueja.Bug:
                    return "Error / Bug";
                case CategoriaQueja.Contenido:
                    return "Contenido";
                case CategoriaQueja.Cuenta:
                    return "Cuenta";
                case CategoriaQueja.Sugerencia:
                    return "Sugerencia";
                default:
                    return "Otro";
            }
        }

        private string GetNombreEstado(EstadoQueja estado)
        {
            switch (estado)
            {
                case EstadoQueja.EnRevision:
                    return "En revisión";
                case EstadoQueja.Resuelta:
                    return "Resuelta";
                default:
                    return "Pendiente";
            }
        }

        #endregion
    }
}
