using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AP.Core.Business;
using AP.Data.Entities;
using AP.Models.Juegos;

namespace AP.MVC.Controllers
{
    // Proteger todo el controlador: solo el rol Admin puede dar mantenimiento a los retos.
    [Authorize(Roles = "Admin")]
    public class RetoController : BaseController
    {
        private readonly RetoBusiness _retoBusiness;

        public RetoController()
        {
            _retoBusiness = new RetoBusiness();
        }

        // GET: Reto
        public ActionResult Index()
        {
            var retos = _retoBusiness.GetRetos();
            var viewModels = retos.Select(MapToViewModel).ToList();
            
            return View(viewModels);
        }

        // GET: Reto/Create
        public ActionResult Create()
        {
            return View(new RetoViewModel { Activo = true });
        }

        // POST: Reto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RetoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var reto = MapToEntity(model);
            
            // Auditoría: seteamos quién está creando el registro
            reto.CreatedBy = User.Identity.Name;

            _retoBusiness.SaveOrUpdate(reto);
            
            return RedirectToAction("Index");
        }

        // GET: Reto/Edit/5
        public ActionResult Edit(int id)
        {
            var reto = _retoBusiness.GetRetos(id).FirstOrDefault();
            if (reto == null)
            {
                return HttpNotFound();
            }

            var model = MapToViewModel(reto);
            return View(model);
        }

        // POST: Reto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RetoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Extraer el primer error del ModelState para mostrarlo
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                if (firstError != null)
                {
                    TempData["ErrorMessage"] = "Error de validación: " + firstError.ErrorMessage;
                }
                return View(model);
            }

            // Buscar la entidad existente para no perder CreatedAt, CreatedBy, etc.
            var reto = _retoBusiness.GetRetos(model.RetoId).FirstOrDefault();
            if (reto == null)
            {
                return HttpNotFound();
            }

            // Actualizar solo los campos modificables
            reto.Titulo = model.Titulo;
            reto.Modo = (ModoJuego)model.Modo;
            reto.Enunciado = model.Enunciado;
            reto.RespuestaCorrecta = model.RespuestaCorrecta;
            reto.Dificultad = model.Dificultad;
            reto.TiempoLimiteSegundos = model.TiempoLimiteSegundos;
            reto.Activo = model.Activo;
            
            // Auditoría: seteamos quién está modificando el registro
            reto.ModifiedBy = User.Identity.Name;

            _retoBusiness.SaveOrUpdate(reto);

            return RedirectToAction("Index");
        }

        // POST: Reto/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _retoBusiness.Delete(id);
            return RedirectToAction("Index");
        }

        #region Mapeo Manual

        private RetoViewModel MapToViewModel(Reto entity)
        {
            return new RetoViewModel
            {
                RetoId = entity.RetoId,
                Titulo = entity.Titulo,
                Modo = (int)entity.Modo,
                Enunciado = entity.Enunciado,
                RespuestaCorrecta = entity.RespuestaCorrecta,
                Dificultad = entity.Dificultad,
                TiempoLimiteSegundos = entity.TiempoLimiteSegundos,
                Activo = entity.Activo
            };
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
