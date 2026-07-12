using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP.Models.Juegos
{
    public class ResultadoJuegoViewModel
    {
        public bool Acertado { get; set; }
        public string RespuestaCorrecta { get; set; }
        public int XpGanado { get; set; }
        public int ExperienciaTotal { get; set; }
        public int Nivel { get; set; }
        public bool SubioDeNivel { get; set; }
    }
}
