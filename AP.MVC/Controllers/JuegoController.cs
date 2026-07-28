using AP.Models.Juegos;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AP.MVC.Controllers
{
    // Pantalla intermedia entre el menu y los modos: antes "Jugar" entraba
    // directo a Operador Perdido y no habia forma de llegar a los demas.
    [Authorize]
    public class JuegoController : BaseController
    {
        // GET: Juego
        public ActionResult Index()
        {
            return View(ObtenerModos());
        }

        // Catalogo que alimenta las tarjetas. Secuencias Logicas ya tiene su
        // estrategia en AP.Core y sus retos en la base, pero le falta el
        // controlador y las vistas, por eso se muestra sin enlace.
        private static List<ModoJuegoViewModel> ObtenerModos()
        {
            return new List<ModoJuegoViewModel>
            {
                new ModoJuegoViewModel
                {
                    Nombre = "Operador Perdido",
                    Descripcion = "Descubre cuál es el operador matemático (+, -, *, /) " +
                                  "que falta para que la ecuación tenga sentido. ¡Piensa rápido!",
                    Icono = "bi-patch-question",
                    Controlador = "OperadorPerdido",
                    Disponible = true
                },
                new ModoJuegoViewModel
                {
                    Nombre = "Contrarreloj",
                    Descripcion = "Resuelve la mayor cantidad de retos matemáticos antes de que " +
                                  "se acabe el tiempo. La presión está al máximo.",
                    Icono = "bi-stopwatch",
                    Controlador = "Contrarreloj",
                    Disponible = true
                },
                new ModoJuegoViewModel
                {
                    Nombre = "Secuencias Lógicas",
                    Descripcion = "Encuentra el patrón oculto en una serie de números y descubre " +
                                  "cuál es el siguiente paso lógico.",
                    Icono = "bi-sort-numeric-down",
                    Controlador = null,
                    Disponible = false
                }
            };
        }
    }
}
