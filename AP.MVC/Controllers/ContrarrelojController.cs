using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Models.Juegos;
using AP.MVC.Models;
using Microsoft.AspNet.Identity;
using AP.Data;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    [Authorize]
    public class ContrarrelojController : BaseController
    {
        private const string UltimoRetoSessionKey = "Contrarreloj_UltimoRetoId";
        private const string ResultadoTempDataKey = "Contrarreloj_Resultado";

        private readonly MathemaXContext _context;
        private readonly PartidaBusiness _juegoBusiness;
        private readonly ExperienciaBusiness _experienciaBusiness;

        public ContrarrelojController()
        {
            _context = new MathemaXContext();
            _juegoBusiness = new PartidaBusiness(_context);
            _experienciaBusiness = new ExperienciaBusiness();
        }

        public ActionResult Jugar()
        {
            TempData.Remove(ResultadoTempDataKey);

            try
            {
                int? ultimoRetoId = Session[UltimoRetoSessionKey] is int id ? id : (int?)null;
                var reto = _juegoBusiness.ObtenerRetoAleatorio(ModoJuego.Contrarreloj, ultimoRetoId);

                Session[ObtenerHoraInicioSessionKey(reto.RetoId)] = DateTime.UtcNow;
                Session[UltimoRetoSessionKey] = reto.RetoId;

                return View(MapToJuegoViewModel(reto));
            }
            catch (AppException ex)
            {
                return View(new RetoJuegoViewModel { MensajeEstadoVacio = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Responder(ResponderContrarrelojViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe ingresar una respuesta antes de enviar.";
                return RedirectToAction("Jugar");
            }

            var usuarioId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["ErrorMessage"] = "No se pudo validar el usuario. Inicie sesión nuevamente.";
                return RedirectToAction("Jugar");
            }

            var horaInicioSessionKey = ObtenerHoraInicioSessionKey(model.RetoId);
            if (!(Session[horaInicioSessionKey] is DateTime horaInicio))
            {
                TempData["ErrorMessage"] = "El reto expiró. Intente con uno nuevo.";
                return RedirectToAction("Jugar");
            }

            Session.Remove(horaInicioSessionKey);

            try
            {
                var segundosTranscurridos = (DateTime.UtcNow - horaInicio).TotalSeconds;
                var tiempoEmpleadoSegundos = Math.Max(0, (int)Math.Ceiling(segundosTranscurridos));
                ResultadoReto resultado;
                ResultadoExperiencia resultadoXp;

                var connectionString = ConfigurationManager.ConnectionStrings["MathemaXContext"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new AppException("No se encontró la configuración de la base de datos.");

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                    using (var juegoContext = new MathemaXContext(connection, false))
                    using (var identityContext = new ApplicationDbContext(connection, false))
                    {
                        try
                        {
                            juegoContext.Database.UseTransaction(transaction);
                            identityContext.Database.UseTransaction(transaction);

                            var usuario = await identityContext.Users.SingleOrDefaultAsync(u => u.Id == usuarioId);
                            if (usuario == null)
                                throw new AppException("No se pudo validar el usuario. Inicie sesión nuevamente.");

                            var juegoBusiness = new PartidaBusiness(juegoContext);
                            resultado = juegoBusiness.ResolverReto(
                                model.RetoId,
                                usuarioId,
                                model.RespuestaUsuario,
                                tiempoEmpleadoSegundos,
                                ModoJuego.Contrarreloj);

                            resultadoXp = _experienciaBusiness.AplicarResultadoPartida(
                                usuario.ExperienciaTotal,
                                usuario.Nivel,
                                resultado.XpGanado);

                            var requiereActualizarUsuario =
                                usuario.ExperienciaTotal != resultadoXp.ExperienciaTotal ||
                                usuario.Nivel != resultadoXp.NivelNuevo;

                            if (requiereActualizarUsuario)
                            {
                                usuario.ExperienciaTotal = resultadoXp.ExperienciaTotal;
                                usuario.Nivel = resultadoXp.NivelNuevo;

                                var filasActualizadas = await identityContext.SaveChangesAsync();
                                if (filasActualizadas != 1)
                                    throw new AppException("No se pudo actualizar la experiencia del usuario.");
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                Session[UltimoRetoSessionKey] = resultado.Reto.RetoId;
                TempData[ResultadoTempDataKey] = new ResultadoJuegoViewModel
                {
                    Acertado = resultado.Acertado,
                    RespuestaCorrecta = resultado.RespuestaCorrecta,
                    XpGanado = resultado.XpGanado,
                    ExperienciaTotal = resultadoXp.ExperienciaTotal,
                    Nivel = resultadoXp.NivelNuevo,
                    SubioDeNivel = resultadoXp.SubioDeNivel,
                    FueraDeTiempo = resultado.FueraDeTiempo
                };

                return RedirectToAction("Resultado");
            }
            catch (AppException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Jugar");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "No se pudo registrar el resultado. Intente con un reto nuevo.";
                return RedirectToAction("Jugar");
            }
        }

        public ActionResult Resultado()
        {
            var model = TempData.Peek(ResultadoTempDataKey) as ResultadoJuegoViewModel;
            if (model == null)
                return RedirectToAction("Jugar");

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }

        private static RetoJuegoViewModel MapToJuegoViewModel(Reto reto)
        {
            return new RetoJuegoViewModel
            {
                RetoId = reto.RetoId,
                Enunciado = reto.Enunciado,
                Dificultad = reto.Dificultad,
                TiempoLimiteSegundos = reto.TiempoLimiteSegundos
            };
        }

        private static string ObtenerHoraInicioSessionKey(int retoId)
        {
            return $"Contrarreloj_HoraInicio_{retoId}";
        }
    }
}
