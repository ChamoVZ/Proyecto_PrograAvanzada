namespace AP.Models.Juegos
{
    public class MarcadoresPaginaViewModel
    {
        public ResultadoPaginado<MarcadorViewModel> Ranking { get; set; }
        public ResultadoPaginado<HistorialPartidaViewModel> Historial { get; set; }
        public FiltroHistorialViewModel Filtros { get; set; }
    }
}
