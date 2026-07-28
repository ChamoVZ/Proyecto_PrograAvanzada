using System.Web.Mvc;
using AP.Core.Exceptions;

namespace AP.MVC.Controllers
{
    /// <summary>
    /// Controlador base del que heredan todos los controladores de dominio.
    /// Su propósito principal es interceptar las excepciones de negocio (AppException)
    /// y mostrarlas amigablemente al usuario, en lugar de mostrar la pantalla amarilla de error.
    /// </summary>
    // DP: MVC - los controladores solo coordinan; la logica vive en Business y los datos en las vistas.
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            if (filterContext.Exception is AppException appEx)
            {
                // Guardamos el mensaje de error para mostrarlo en la vista actual
                TempData["ErrorMessage"] = appEx.Message;
                
                filterContext.ExceptionHandled = true;

                // Redirigimos a la acción que generó el error usando el Referrer si es posible,
                // o al Index por defecto.
                var referrer = filterContext.HttpContext.Request.UrlReferrer;
                if (referrer != null)
                {
                    filterContext.Result = new RedirectResult(referrer.ToString());
                }
                else
                {
                    filterContext.Result = RedirectToAction("Index");
                }
            }
            // Si es otro tipo de excepción (falla de base de datos, null reference, etc.)
            // dejamos que ASP.NET maneje el error normalmente (Yellow Screen).

            base.OnException(filterContext);
        }
    }
}
