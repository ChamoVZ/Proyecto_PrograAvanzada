using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP.Core.Business
{
    public class ResultadoExperiencia
    {
        public int XpGanado { get; set; }
        public int ExperienciaTotal { get; set; }
        public int NivelAnterior { get; set; }
        public int NivelNuevo { get; set; }
        public bool SubioDeNivel => NivelNuevo > NivelAnterior;
    }

    // SOLID: SRP - solo calcula XP y niveles; no toca persistencia ni conoce repositorios.
    public class ExperienciaBusiness
    {
        private const int XpPorPuntoDeDificultad = 10;
        private const int XpBasePorNivel = 100;

        public int CalcularXpGanado(int dificultad, bool acertado)
        {
            if (dificultad < 1 || dificultad > 5)
                throw new ArgumentOutOfRangeException(nameof(dificultad), "La dificultad debe estar entre 1 y 5.");

            if (!acertado)
                return 0;

            return dificultad * XpPorPuntoDeDificultad;
        }

        public int XpRequeridoParaNivel(int nivel)
        {
            if (nivel <= 1) return 0;

            int nivelesPrevios = nivel - 1;
            return XpBasePorNivel * nivelesPrevios * (nivelesPrevios + 1) / 2;
        }

        public int CalcularNivelPorXp(int experienciaTotal)
        {
            int nivel = 1;
            while (experienciaTotal >= XpRequeridoParaNivel(nivel + 1))
            {
                nivel++;
            }
            return nivel;
        }

        public ResultadoExperiencia AplicarResultadoPartida(int experienciaActual, int nivelActual, int xpGanado)
        {
            int nuevaExperiencia = experienciaActual + xpGanado;
            int nuevoNivel = CalcularNivelPorXp(nuevaExperiencia);

            return new ResultadoExperiencia
            {
                XpGanado = xpGanado,
                ExperienciaTotal = nuevaExperiencia,
                NivelAnterior = nivelActual,
                NivelNuevo = nuevoNivel,
            };
        }
    }
}
