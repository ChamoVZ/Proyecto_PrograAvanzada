using AP.Models.Juegos;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    [Authorize]
    public class JuegoController : BaseController
    {
        public ActionResult Index()
        {
            return View(ObtenerModos());
        }

        private static List<ModoJuegoViewModel> ObtenerModos()
        {
            return new List<ModoJuegoViewModel>
            {
                new ModoJuegoViewModel
                {
                    Nombre = "Operador Perdido",
                    Descripcion = "Descubra cuál es el operador matemático (+, -, *, /) " +
                                  "que falta para que la ecuación tenga sentido. ¡Piense rápido!",
                    Icono = "bi-patch-question",
                    Controlador = "OperadorPerdido"
                },
                new ModoJuegoViewModel
                {
                    Nombre = "Contrarreloj",
                    Descripcion = "Resuelva la mayor cantidad de retos matemáticos antes de que " +
                                  "se acabe el tiempo. La presión está al máximo.",
                    Icono = "bi-stopwatch",
                    Controlador = "Contrarreloj"
                },
                new ModoJuegoViewModel
                {
                    Nombre = "Secuencias Lógicas",
                    Descripcion = "Encuentre el patrón oculto en una serie de números y descubra " +
                                  "cuál es el siguiente paso lógico.",
                    Icono = "bi-sort-numeric-down",
                    Controlador = "SecuenciasLogicas"
                }
            };
        }
    }
}
