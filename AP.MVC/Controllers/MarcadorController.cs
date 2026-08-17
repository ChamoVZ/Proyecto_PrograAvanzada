using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data;
using AP.Data.Entities;
using AP.Models.Juegos;
using AP.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AP.MVC.Controllers
{
    [Authorize]
    public class MarcadorController : BaseController
    {
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

        public ActionResult Index(
            FiltroHistorialViewModel filtros,
            int paginaRanking = 1,
            int tamanoRanking = 10,
            int paginaHistorial = 1,
            int tamanoHistorial = 10)
        {
            filtros = filtros ?? new FiltroHistorialViewModel();
            var ranking = _partidaBusiness.GetRankingGlobal(paginaRanking, tamanoRanking);

            // El ranking sale de Partidas (AP.Core), pero el nombre del jugador vive en
            // AspNetUsers, que solo es visible desde AP.MVC; por eso el cruce se hace aqui.
            // Los nombres se resuelven de una sola vez: uno por fila era una consulta por jugador.
            var idsDelRanking = ranking.Elementos.Select(r => r.UsuarioId).ToList();
            var nombresPorId = UserManager.Users
                .Where(u => idsDelRanking.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName);

            var marcadores = new List<MarcadorViewModel>();
            var posicion = ((ranking.PaginaActual - 1) * ranking.TamanoPagina) + 1;
            foreach (var fila in ranking.Elementos)
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

            var historial = ObtenerHistorial(filtros, paginaHistorial, tamanoHistorial);

            var model = new MarcadoresPaginaViewModel
            {
                Ranking = new AP.Models.ResultadoPaginado<MarcadorViewModel>
                {
                    Elementos = marcadores,
                    PaginaActual = ranking.PaginaActual,
                    TamanoPagina = ranking.TamanoPagina,
                    TotalRegistros = ranking.TotalRegistros,
                    TotalPaginas = ranking.TotalPaginas
                },
                Historial = historial,
                Filtros = filtros
            };

            return View(model);
        }

        [HttpGet]
        public JsonResult Historial(
            FiltroHistorialViewModel filtros,
            int paginaHistorial = 1,
            int tamanoHistorial = 10)
        {
            try
            {
                filtros = filtros ?? new FiltroHistorialViewModel();
                var historial = ObtenerHistorial(filtros, paginaHistorial, tamanoHistorial);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                return Json(new
                {
                    correcto = true,
                    elementos = historial.Elementos.Select(partida => new
                    {
                        fechaJuego = partida.FechaJuego.ToString("dd/MM/yyyy HH:mm"),
                        partida.TituloReto,
                        partida.Modo,
                        partida.Acertado,
                        partida.TiempoEmpleadoSegundos,
                        partida.XpGanado
                    }),
                    paginaActual = historial.PaginaActual,
                    tamanoPagina = historial.TamanoPagina,
                    totalRegistros = historial.TotalRegistros,
                    totalPaginas = historial.TotalPaginas
                }, JsonRequestBehavior.AllowGet);
            }
            catch (AppException ex)
            {
                Response.StatusCode = 400;
                return Json(new { correcto = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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

        private AP.Models.ResultadoPaginado<HistorialPartidaViewModel> ObtenerHistorial(
            FiltroHistorialViewModel filtros,
            int pagina,
            int tamanoPagina)
        {
            var historial = _partidaBusiness.GetHistorial(
                User.Identity.GetUserId(),
                filtros.Resultado,
                filtros.Modo,
                filtros.Desde,
                filtros.Hasta,
                pagina,
                tamanoPagina);

            return new AP.Models.ResultadoPaginado<HistorialPartidaViewModel>
            {
                Elementos = historial.Elementos.Select(MapToHistorialViewModel).ToList(),
                PaginaActual = historial.PaginaActual,
                TamanoPagina = historial.TamanoPagina,
                TotalRegistros = historial.TotalRegistros,
                TotalPaginas = historial.TotalPaginas
            };
        }

        private HistorialPartidaViewModel MapToHistorialViewModel(HistorialPartida entity)
        {
            return new HistorialPartidaViewModel
            {
                FechaJuego = entity.FechaJuego,
                Acertado = entity.Acertado,
                TiempoEmpleadoSegundos = entity.TiempoEmpleadoSegundos,
                XpGanado = entity.XpGanado,
                Modo = GetNombreModo(entity.Modo),
                TituloReto = entity.TituloReto
            };
        }

        private string GetNombreModo(ModoJuego modo)
        {
            switch (modo)
            {
                case ModoJuego.OperadorPerdido:
                    return "Operador Perdido";
                case ModoJuego.SecuenciasLogicas:
                    return "Secuencias Lógicas";
                default:
                    return "Contrarreloj";
            }
        }

        #endregion
    }
}
