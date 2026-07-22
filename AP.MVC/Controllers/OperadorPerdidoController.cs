using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Models.Juegos;
using AP.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    // Cualquier usuario autenticado puede jugar (no hace falta rol Admin).
    [Authorize]
    public class OperadorPerdidoController : BaseController
    {
        private const string UltimoRetoSessionKey = "OperadorPerdido_UltimoRetoId";
        private const string ResultadoTempDataKey = "OperadorPerdido_Resultado";

        private readonly PartidaBusiness _juegoBusiness;
        private readonly ExperienciaBusiness _experienciaBusiness;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public OperadorPerdidoController()
        {
            _juegoBusiness = new PartidaBusiness();
            _experienciaBusiness = new ExperienciaBusiness();
        }

        // GET: OperadorPerdido/Jugar
        public ActionResult Jugar()
        {
            TempData.Remove(ResultadoTempDataKey);

            try
            {
                int? ultimoRetoId = Session[UltimoRetoSessionKey] is int id ? id : (int?)null;
                var reto = _juegoBusiness.ObtenerRetoAleatorio(ModoJuego.OperadorPerdido, ultimoRetoId);

                Session[ObtenerHoraInicioSessionKey(reto.RetoId)] = DateTime.UtcNow;
                Session[UltimoRetoSessionKey] = reto.RetoId;

                var model = MapToJuegoViewModel(reto);
                return View(model);
            }
            catch (AppException ex)
            {
                return View(new RetoJuegoViewModel
                {
                    MensajeEstadoVacio = ex.Message
                });
            }
        }

        // POST: OperadorPerdido/Responder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Responder(ResponderRetoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una respuesta antes de enviar.";
                return RedirectToAction("Jugar");
            }

            var horaInicioSessionKey = ObtenerHoraInicioSessionKey(model.RetoId);
            if (!(Session[horaInicioSessionKey] is DateTime horaInicio))
            {
                TempData["ErrorMessage"] = "El reto expiró. Intente con uno nuevo.";
                return RedirectToAction("Jugar");
            }

            // Evita enviar el mismo reto dos veces.
            Session.Remove(horaInicioSessionKey);

            var segundosTranscurridos = (DateTime.UtcNow - horaInicio).TotalSeconds;
            var tiempoEmpleadoSegundos = Math.Max(0, (int)Math.Ceiling(segundosTranscurridos));

            var resultado = _juegoBusiness.ResolverReto(
                model.RetoId,
                User.Identity.GetUserId(),
                model.RespuestaUsuario,
                tiempoEmpleadoSegundos);

            Session[UltimoRetoSessionKey] = resultado.Reto.RetoId;

            // Actualizar XP y Nivel del usuario (ApplicationUser vive en AP.MVC,
            // por eso esto no puede hacerse desde AP.Core/AP.Repositories).
            var usuario = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var resultadoXp = _experienciaBusiness.AplicarResultadoPartida(
                usuario.ExperienciaTotal,
                usuario.Nivel,
                resultado.XpGanado);

            usuario.ExperienciaTotal = resultadoXp.ExperienciaTotal;
            usuario.Nivel = resultadoXp.NivelNuevo;
            await UserManager.UpdateAsync(usuario);

            var viewModel = new ResultadoJuegoViewModel
            {
                Acertado = resultado.Acertado,
                RespuestaCorrecta = resultado.RespuestaCorrecta,
                XpGanado = resultado.XpGanado,
                ExperienciaTotal = resultadoXp.ExperienciaTotal,
                Nivel = resultadoXp.NivelNuevo,
                SubioDeNivel = resultadoXp.SubioDeNivel,
                FueraDeTiempo = resultado.FueraDeTiempo
            };

            TempData[ResultadoTempDataKey] = viewModel;
            return RedirectToAction("Resultado");
        }

        // GET: OperadorPerdido/Resultado
        public ActionResult Resultado()
        {
            // Mantiene el resultado al refrescar.
            var model = TempData.Peek(ResultadoTempDataKey) as ResultadoJuegoViewModel;
            if (model == null)
                return RedirectToAction("Jugar");

            return View(model);
        }

        #region Mapeo Manual

        private RetoJuegoViewModel MapToJuegoViewModel(Reto entity)
        {
            return new RetoJuegoViewModel
            {
                RetoId = entity.RetoId,
                Enunciado = entity.Enunciado,
                Dificultad = entity.Dificultad,
                TiempoLimiteSegundos = entity.TiempoLimiteSegundos
            };
        }

        private static string ObtenerHoraInicioSessionKey(int retoId)
        {
            return $"OperadorPerdido_HoraInicio_{retoId}";
        }

        #endregion
    }
}
