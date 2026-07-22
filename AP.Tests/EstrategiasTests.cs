using AP.Core.Business.Estrategias;
using AP.Data.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class EstrategiasTests
    {
        private static Reto NuevoReto(ModoJuego modo, string respuesta, int dificultad, int tiempoLimite)
        {
            return new Reto
            {
                RetoId = 1,
                Modo = modo,
                Enunciado = "Reto de prueba",
                RespuestaCorrecta = respuesta,
                Dificultad = dificultad,
                TiempoLimiteSegundos = tiempoLimite,
                Activo = true
            };
        }

        [TestMethod]
        public void OperadorPerdido_ValidarRespuesta_IgnoraMayusculasYEspacios()
        {
            var estrategia = new OperadorPerdidoStrategy();
            var reto = NuevoReto(ModoJuego.OperadorPerdido, "*", 1, 30);

            Assert.IsTrue(estrategia.ValidarRespuesta(reto, "  *  "));
            Assert.IsFalse(estrategia.ValidarRespuesta(reto, "/"));
        }

        [TestMethod]
        public void OperadorPerdido_CalcularXp_UsaXpBasePorDificultad()
        {
            var estrategia = new OperadorPerdidoStrategy();
            var reto = NuevoReto(ModoJuego.OperadorPerdido, "*", 3, 30);

            Assert.AreEqual(30, estrategia.CalcularXp(reto, true, 10));
            Assert.AreEqual(0, estrategia.CalcularXp(reto, false, 10));
        }

        [TestMethod]
        public void Contrarreloj_CalcularXp_SumaBonoPorRapidez()
        {
            var estrategia = new ContrarrelojStrategy();
            var reto = NuevoReto(ModoJuego.Contrarreloj, "83", 2, 20);

            // XP base 20 + bono (20 - 10) / 5 = 2.
            Assert.AreEqual(22, estrategia.CalcularXp(reto, true, 10));
        }

        [TestMethod]
        public void Contrarreloj_CalcularXp_SinAcierto_RetornaCero()
        {
            var estrategia = new ContrarrelojStrategy();
            var reto = NuevoReto(ModoJuego.Contrarreloj, "83", 2, 20);

            Assert.AreEqual(0, estrategia.CalcularXp(reto, false, 5));
        }

        [TestMethod]
        public void Contrarreloj_CalcularXp_TiempoExcedido_NoRestaXp()
        {
            var estrategia = new ContrarrelojStrategy();
            var reto = NuevoReto(ModoJuego.Contrarreloj, "83", 2, 20);

            // El bono nunca es negativo aunque tarde mas del limite.
            Assert.AreEqual(20, estrategia.CalcularXp(reto, true, 100));
        }

        [TestMethod]
        public void SecuenciasLogicas_ValidarRespuesta_ComparaComoEntero()
        {
            var estrategia = new SecuenciasLogicasStrategy();
            var reto = NuevoReto(ModoJuego.SecuenciasLogicas, "32", 2, 40);

            Assert.IsTrue(estrategia.ValidarRespuesta(reto, " 32 "));
            Assert.IsFalse(estrategia.ValidarRespuesta(reto, "treinta y dos"));
        }

        [TestMethod]
        public void SecuenciasLogicas_CalcularXp_PremiaMasQueLosOtrosModos()
        {
            var estrategia = new SecuenciasLogicasStrategy();
            var reto = NuevoReto(ModoJuego.SecuenciasLogicas, "32", 2, 40);

            Assert.AreEqual(24, estrategia.CalcularXp(reto, true, 10));
            Assert.AreEqual(0, estrategia.CalcularXp(reto, false, 10));
        }
    }
}
