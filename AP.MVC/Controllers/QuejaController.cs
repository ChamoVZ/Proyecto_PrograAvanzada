using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Models.Buzon;
using Microsoft.AspNet.Identity;

namespace AP.MVC.Controllers
{
    [Authorize]
    public class QuejaController : BaseController
    {
        private readonly QuejaBusiness _quejaBusiness;

        public QuejaController()
        {
            _quejaBusiness = new QuejaBusiness();
        }

        // GET: Queja
        public ActionResult Index()
        {
            var usuarioId = User.Identity.GetUserId();

            // Admin y Support ven todas las quejas; el jugador solo ve las suyas.
            var quejas = User.IsInRole("Admin") || User.IsInRole("Support")
                ? _quejaBusiness.GetTodas()
                : _quejaBusiness.GetPorUsuario(usuarioId);

            var viewModels = quejas
                .OrderByDescending(q => q.FechaCreacion)
                .Select(MapToViewModel)
                .ToList();

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

        #region Mapeo Manual

        private QuejaViewModel MapToViewModel(Queja entity)
        {
            return new QuejaViewModel
            {
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
