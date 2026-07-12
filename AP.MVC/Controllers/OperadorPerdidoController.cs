using AP.Core.Business;
using AP.Data.Entities;
using AP.Models.Juegos;
using AP.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    // Cualquier usuario autenticado puede jugar (no hace falta rol Admin).
    [Authorize]
    public class OperadorPerdidoController : BaseController
    {
        private readonly OperadorPerdidoBusiness _juegoBusiness;
        private readonly ExperienciaBusiness _experienciaBusiness;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public OperadorPerdidoController()
        {
            _juegoBusiness = new OperadorPerdidoBusiness();
            _experienciaBusiness = new ExperienciaBusiness();
        }

        // GET: OperadorPerdido/Jugar
        public ActionResult Jugar()
        {
            var reto = _juegoBusiness.ObtenerRetoAleatorio();
            var model = MapToJuegoViewModel(reto);
            return View(model);
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

            // Tiempo empleado = ahora menos el momento en que se mostró el reto
            var horaInicio = new System.DateTime(model.HoraInicioTicks);
            var tiempoEmpleadoSegundos = (int)(System.DateTime.Now - horaInicio).TotalSeconds;

            var resultado = _juegoBusiness.ResolverReto(
                model.RetoId,
                User.Identity.GetUserId(),
                model.RespuestaUsuario,
                tiempoEmpleadoSegundos);

            // Actualizar XP y Nivel del usuario (ApplicationUser vive en AP.MVC,
            // por eso esto no puede hacerse desde AP.Core/AP.Repositories).
            var usuario = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var resultadoXp = _experienciaBusiness.AplicarResultadoPartida(
                usuario.ExperienciaTotal,
                usuario.Nivel,
                resultado.Reto.Dificultad,
                resultado.Acertado);

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
                SubioDeNivel = resultadoXp.SubioDeNivel
            };

            return View("Resultado", viewModel);
        }

        #region Mapeo Manual

        private RetoJuegoViewModel MapToJuegoViewModel(Reto entity)
        {
            return new RetoJuegoViewModel
            {
                RetoId = entity.RetoId,
                Enunciado = entity.Enunciado,
                Dificultad = entity.Dificultad,
                TiempoLimiteSegundos = entity.TiempoLimiteSegundos,
                HoraInicioTicks = System.DateTime.Now.Ticks
            };
        }

        #endregion
    }
}