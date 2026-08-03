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

            var viewModels = solicitudes
                .OrderByDescending(s => s.FechaCreacion)
                .Select(MapToViewModel)
                .ToList();

            return View(viewModels);
        }

        // GET: SolicitudTI/Create
        public ActionResult Create()
        {
            CargarRespuestaAsistente();
            return View(new SolicitudTIViewModel { Estado = (int)EstadoSolicitud.Abierta });
        }

        // POST: SolicitudTI/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitudTIViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CargarRespuestaAsistente();
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
                CargarRespuestaAsistente();
                return View(model);
            }
            catch (System.Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al crear la solicitud. Intente más tarde.";
                CargarRespuestaAsistente();
                return View(model);
            }
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

        private void CargarRespuestaAsistente()
        {
            ViewBag.RespuestaAsistente = _chatbotService.GetRespuesta(string.Empty);
        }

        #region Mapeo Manual

        private SolicitudTIViewModel MapToViewModel(SolicitudTI entity)
        {
            return new SolicitudTIViewModel
            {
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
