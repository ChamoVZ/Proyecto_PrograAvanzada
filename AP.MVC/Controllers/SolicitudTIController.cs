using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Models.Soporte;
using AP.Services;
using Microsoft.AspNet.Identity;

namespace AP.MVC.Controllers
{
    [Authorize]
    public class SolicitudTIController : BaseController
    {
        private readonly SolicitudTIBusiness _solicitudTIBusiness;
        private readonly ChatbotService _chatbotService;

        public SolicitudTIController()
        {
            _solicitudTIBusiness = new SolicitudTIBusiness();
            _chatbotService = new ChatbotService();
        }

        // GET: SolicitudTI
        public ActionResult Index()
        {
            var usuarioId = User.Identity.GetUserId();
            var solicitudes = User.IsInRole("Admin") || User.IsInRole("Support")
                ? _solicitudTIBusiness.GetTodas()
                : _solicitudTIBusiness.GetPorUsuario(usuarioId);

            var esAdmin = User.IsInRole("Admin");
            var viewModels = solicitudes
                .OrderByDescending(s => s.FechaCreacion)
                .Select(s => MapToViewModel(s, usuarioId, esAdmin))
                .ToList();

            return View(viewModels);
        }

        // GET: SolicitudTI/Create
        public ActionResult Create()
        {
            var model = new SolicitudTIViewModel { Estado = (int)EstadoSolicitud.Abierta };

            CargarRespuestaAsistente(model);
            return View(model);
        }

        // POST: SolicitudTI/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitudTIViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CargarRespuestaAsistente(model);
                return View(model);
            }

            try
            {
                var solicitud = MapToEntity(model);
                solicitud.UsuarioId = User.Identity.GetUserId();
                solicitud.CreatedBy = User.Identity.Name;

                _solicitudTIBusiness.Save(solicitud);

                return RedirectToAction("Index");
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                CargarRespuestaAsistente(model);
                return View(model);
            }
            catch (System.Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al crear la solicitud. Intente más tarde.";
                CargarRespuestaAsistente(model);
                return View(model);
            }
        }

        // GET: SolicitudTI/Edit/5
        public ActionResult Edit(int id)
        {
            var solicitud = _solicitudTIBusiness.GetPorId(id);
            if (solicitud == null || !solicitud.Activo)
            {
                return HttpNotFound();
            }

            var usuarioId = User.Identity.GetUserId();
            if (!_solicitudTIBusiness.PuedeEditar(solicitud, usuarioId))
            {
                TempData["ErrorMessage"] = "Solo el autor puede editar su solicitud, y solo mientras siga abierta.";
                return RedirectToAction("Index");
            }

            return View(MapToViewModel(solicitud, usuarioId, User.IsInRole("Admin")));
        }

        // POST: SolicitudTI/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SolicitudTIViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var solicitud = _solicitudTIBusiness.GetPorId(model.SolicitudTIId);
            if (solicitud == null || !solicitud.Activo)
            {
                return HttpNotFound();
            }

            try
            {
                // Recargamos la entidad y solo tocamos lo editable, asi no perdemos UsuarioId,
                // el estado que puso soporte ni la auditoria de creacion.
                solicitud.Asunto = model.Asunto;
                solicitud.Descripcion = model.Descripcion;
                solicitud.ModifiedBy = User.Identity.Name;

                _solicitudTIBusiness.Actualizar(solicitud, User.Identity.GetUserId());

                return RedirectToAction("Index");
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        // POST: SolicitudTI/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _solicitudTIBusiness.Desactivar(id, User.Identity.GetUserId(), User.IsInRole("Admin"));
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: SolicitudTI/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Support")]
        public ActionResult CambiarEstado(int id, int estado)
        {
            try
            {
                _solicitudTIBusiness.CambiarEstado(id, (EstadoSolicitud)estado);
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private void CargarRespuestaAsistente(SolicitudTIViewModel model)
        {
            // El asistente orienta sobre lo que el usuario ya escribió; en el alta nueva
            // los dos campos vienen vacíos y responde con la ayuda genérica.
            var texto = model.Asunto + " " + model.Descripcion;

            ViewBag.RespuestaAsistente = _chatbotService.GetRespuesta(texto);
        }

        #region Mapeo Manual

        private SolicitudTIViewModel MapToViewModel(SolicitudTI entity, string usuarioId, bool esAdmin)
        {
            return new SolicitudTIViewModel
            {
                PuedeEditar = _solicitudTIBusiness.PuedeEditar(entity, usuarioId),
                PuedeEliminar = _solicitudTIBusiness.PuedeEliminar(entity, usuarioId, esAdmin),
                SolicitudTIId = entity.SolicitudTIId,
                UsuarioId = entity.UsuarioId,
                Asunto = entity.Asunto,
                Descripcion = entity.Descripcion,
                Estado = (int)entity.Estado,
                FechaCreacion = entity.FechaCreacion,
                NombreSolicitante = entity.CreatedBy ?? "Usuario",
                NombreEstado = GetNombreEstado(entity.Estado)
            };
        }

        private SolicitudTI MapToEntity(SolicitudTIViewModel model)
        {
            return new SolicitudTI
            {
                SolicitudTIId = model.SolicitudTIId,
                UsuarioId = model.UsuarioId,
                Asunto = model.Asunto,
                Descripcion = model.Descripcion,
                Estado = (EstadoSolicitud)model.Estado,
                FechaCreacion = model.FechaCreacion
            };
        }

        private string GetNombreEstado(EstadoSolicitud estado)
        {
            switch (estado)
            {
                case EstadoSolicitud.EnProceso:
                    return "En proceso";
                case EstadoSolicitud.Cerrada:
                    return "Cerrada";
                default:
                    return "Abierta";
            }
        }

        #endregion
    }
}
