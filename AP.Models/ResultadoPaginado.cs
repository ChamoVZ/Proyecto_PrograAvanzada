using System.Collections.Generic;

namespace AP.Models
{
    public interface IResultadoPaginado
    {
        int PaginaActual { get; }
        int TamanoPagina { get; }
        int TotalRegistros { get; }
        int TotalPaginas { get; }
    }

    public class ResultadoPaginado<T> : IResultadoPaginado
    {
        public IList<T> Elementos { get; set; }
        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
