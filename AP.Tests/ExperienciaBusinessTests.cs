using System;
using AP.Core.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class ExperienciaBusinessTests
    {
        private readonly ExperienciaBusiness _business = new ExperienciaBusiness();

        [TestMethod]
        [DataRow(1, 10)]
        [DataRow(2, 20)]
        [DataRow(3, 30)]
        [DataRow(4, 40)]
        [DataRow(5, 50)]
        public void CalcularXpGanado_RespuestaCorrecta_RetornaXpEsperado(int dificultad, int esperado)
        {
            var resultado = _business.CalcularXpGanado(dificultad, true);

            Assert.AreEqual(esperado, resultado);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(6)]
        public void CalcularXpGanado_DificultadInvalida_LanzaExcepcion(int dificultad)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => _business.CalcularXpGanado(dificultad, true));
        }

        [TestMethod]
        public void CalcularXpGanado_RespuestaIncorrecta_RetornaCero()
        {
            var resultado = _business.CalcularXpGanado(5, false);

            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 100)]
        [DataRow(3, 300)]
        [DataRow(4, 600)]
        public void XpRequeridoParaNivel_NivelValido_RetornaUmbral(int nivel, int esperado)
        {
            var resultado = _business.XpRequeridoParaNivel(nivel);

            Assert.AreEqual(esperado, resultado);
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(99, 1)]
        [DataRow(100, 2)]
        [DataRow(299, 2)]
        [DataRow(300, 3)]
        [DataRow(599, 3)]
        [DataRow(600, 4)]
        public void CalcularNivelPorXp_EnLosBordes_RetornaNivelEsperado(int xp, int esperado)
        {
            var resultado = _business.CalcularNivelPorXp(xp);

            Assert.AreEqual(esperado, resultado);
        }

        [TestMethod]
        public void AplicarResultadoPartida_AlCruzarUmbral_IndicaSubidaDeNivel()
        {
            var resultado = _business.AplicarResultadoPartida(90, 1, 10);

            Assert.AreEqual(100, resultado.ExperienciaTotal);
            Assert.AreEqual(2, resultado.NivelNuevo);
            Assert.IsTrue(resultado.SubioDeNivel);
        }

        [TestMethod]
        public void AplicarResultadoPartida_SinCruzarUmbral_MantieneNivel()
        {
            var resultado = _business.AplicarResultadoPartida(10, 1, 10);

            Assert.AreEqual(20, resultado.ExperienciaTotal);
            Assert.AreEqual(1, resultado.NivelNuevo);
            Assert.IsFalse(resultado.SubioDeNivel);
        }
    }
}
