using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AP.Core.Business;
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
        private readonly PartidaBusiness _partidaBusiness;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public MarcadorController()
        {
            _partidaBusiness = new PartidaBusiness();
        }

        // GET: Marcador
        public ActionResult Index()
        {
            var ranking = _partidaBusiness.GetRankingGlobal().ToList();

            // El ranking sale de Partidas (AP.Core), pero el nombre del jugador vive en
            // AspNetUsers, que solo es visible desde AP.MVC; por eso el cruce se hace aqui.
            var marcadores = new List<MarcadorViewModel>();
            int posicion = 1;
            foreach (var fila in ranking)
            {
                var usuario = UserManager.FindById(fila.UsuarioId);
                marcadores.Add(new MarcadorViewModel
                {
                    Posicion = posicion,
                    NombreJugador = usuario != null ? usuario.UserName : "Jugador",
                    ExperienciaTotal = fila.ExperienciaTotal,
                    PartidasJugadas = fila.PartidasJugadas,
                    Aciertos = fila.Aciertos
                });
                posicion++;
            }

            ViewBag.Historial = _partidaBusiness.GetHistorial(User.Identity.GetUserId()).ToList();
            return View(marcadores);
        }
    }
}
