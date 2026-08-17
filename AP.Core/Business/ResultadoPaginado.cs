using System;
using System.Collections.Generic;
using System.Linq;

namespace AP.Core.Business
{
    public class ResultadoPaginado<T>
    {
        public IList<T> Elementos { get; private set; }
        public int PaginaActual { get; private set; }
        public int TamanoPagina { get; private set; }
        public int TotalRegistros { get; private set; }
        public int TotalPaginas { get; private set; }

        public static ResultadoPaginado<T> Crear(
            int pagina,
            int tamanoPagina,
            int totalRegistros,
            Func<int, int, IEnumerable<T>> cargarPagina)
        {
            var tamanoValido = tamanoPagina == 20 ? 20 : 10;
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoValido);
            var paginaValida = Math.Max(1, pagina);

            if (totalPaginas == 0)
            {
                paginaValida = 1;
            }
            else if (paginaValida > totalPaginas)
            {
                paginaValida = totalPaginas;
            }

            return new ResultadoPaginado<T>
            {
                Elementos = cargarPagina(paginaValida, tamanoValido).ToList(),
                PaginaActual = paginaValida,
                TamanoPagina = tamanoValido,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas
            };
        }
    }
}
