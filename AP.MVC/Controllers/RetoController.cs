using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Data;
using AP.Data.Entities;
using AP.Models.Juegos;

namespace AP.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RetoController : BaseController
    {
        private readonly MathemaXContext _context;
        private readonly RetoBusiness _retoBusiness;

        public RetoController()
        {
            _context = new MathemaXContext();
            _retoBusiness = new RetoBusiness(_context);
        }

        public ActionResult Index(int pagina = 1, int tamanoPagina = 10)
        {
            var retos = _retoBusiness.GetRetosPaginados(pagina, tamanoPagina);
            var viewModels = new AP.Models.ResultadoPaginado<RetoViewModel>
            {
                Elementos = retos.Elementos.Select(MapToViewModel).ToList(),
                PaginaActual = retos.PaginaActual,
                TamanoPagina = retos.TamanoPagina,
                TotalRegistros = retos.TotalRegistros,
                TotalPaginas = retos.TotalPaginas
            };
            
            return View(viewModels);
        }

        public ActionResult Create()
        {
            return View(new RetoViewModel { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RetoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var reto = MapToEntity(model);
            
            reto.CreatedBy = User.Identity.Name;

            _retoBusiness.SaveOrUpdate(reto);
            
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var reto = _retoBusiness.GetRetoPorId(id);
            if (reto == null)
            {
                return HttpNotFound();
            }

            var model = MapToViewModel(reto);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RetoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                if (firstError != null)
                {
                    TempData["ErrorMessage"] = "Error de validación: " + firstError.ErrorMessage;
                }
                return View(model);
            }

            var reto = _retoBusiness.GetRetoPorId(model.RetoId);
            if (reto == null)
            {
                return HttpNotFound();
            }

            reto.Titulo = model.Titulo;
            reto.Modo = (ModoJuego)model.Modo;
            reto.Enunciado = model.Enunciado;
            reto.RespuestaCorrecta = model.RespuestaCorrecta;
            reto.Dificultad = model.Dificultad;
            reto.TiempoLimiteSegundos = model.TiempoLimiteSegundos;
            reto.Activo = model.Activo;
            
            reto.ModifiedBy = User.Identity.Name;

            _retoBusiness.SaveOrUpdate(reto);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(
            int id)
        {
            _retoBusiness.Desactivar(id);
            return RedirectToAction("Index");
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

        private RetoViewModel MapToViewModel(Reto entity)
        {
            return new RetoViewModel
            {
                RetoId = entity.RetoId,
                Titulo = entity.Titulo,
                Modo = (int)entity.Modo,
                NombreModo = GetNombreModo(entity.Modo),
                Enunciado = entity.Enunciado,
                RespuestaCorrecta = entity.RespuestaCorrecta,
                Dificultad = entity.Dificultad,
                TiempoLimiteSegundos = entity.TiempoLimiteSegundos,
                Activo = entity.Activo
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

        private Reto MapToEntity(RetoViewModel model)
        {
            return new Reto
            {
                RetoId = model.RetoId,
                Titulo = model.Titulo,
                Modo = (ModoJuego)model.Modo,
                Enunciado = model.Enunciado,
                RespuestaCorrecta = model.RespuestaCorrecta,
                Dificultad = model.Dificultad,
                TiempoLimiteSegundos = model.TiempoLimiteSegundos,
                Activo = model.Activo
            };
        }

        #endregion
    }
}
