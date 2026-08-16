using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Data;
using AP.Data.Entities;
using AP.Models.Juegos;
using AP.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AP.MVC.Controllers
{
    // Cualquier usuario autenticado puede ver los marcadores.
    [Authorize]
    public class MarcadorController : BaseController
    {
        // El contexto es del controller: asi los dos repositorios comparten uno solo y se libera al final del request.
        private readonly MathemaXContext _context;
        private readonly PartidaBusiness _partidaBusiness;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public MarcadorController()
        {
            _context = new MathemaXContext();
            _partidaBusiness = new PartidaBusiness(_context);
        }

        // GET: Marcador
        public ActionResult Index()
        {
            var ranking = _partidaBusiness.GetRankingGlobal().ToList();

            // El ranking sale de Partidas (AP.Core), pero el nombre del jugador vive en
            // AspNetUsers, que solo es visible desde AP.MVC; por eso el cruce se hace aqui.
            // Los nombres se resuelven de una sola vez: uno por fila era una consulta por jugador.
            var idsDelRanking = ranking.Select(r => r.UsuarioId).ToList();
            var nombresPorId = UserManager.Users
                .Where(u => idsDelRanking.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName);

            var marcadores = new List<MarcadorViewModel>();
            int posicion = 1;
            foreach (var fila in ranking)
            {
                string nombre;
                marcadores.Add(new MarcadorViewModel
                {
                    Posicion = posicion,
                    NombreJugador = nombresPorId.TryGetValue(fila.UsuarioId, out nombre) ? nombre : "Jugador",
                    ExperienciaTotal = fila.ExperienciaTotal,
                    PartidasJugadas = fila.PartidasJugadas,
                    Aciertos = fila.Aciertos
                });
                posicion++;
            }

            ViewBag.Historial = _partidaBusiness.GetHistorial(User.Identity.GetUserId())
                .Select(MapToHistorialViewModel)
                .ToList();

            return View(marcadores);
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

        private HistorialPartidaViewModel MapToHistorialViewModel(Partida entity)
        {
            return new HistorialPartidaViewModel
            {
                FechaJuego = entity.FechaJuego,
                Acertado = entity.Acertado,
                TiempoEmpleadoSegundos = entity.TiempoEmpleadoSegundos,
                XpGanado = entity.XpGanado
            };
        }

        #endregion
    }
}
