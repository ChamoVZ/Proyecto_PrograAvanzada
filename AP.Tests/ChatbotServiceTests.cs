using AP.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class ChatbotServiceTests
    {
        private static string Responder(string mensaje)
        {
            return new ChatbotService().GetRespuesta(mensaje);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void GetRespuesta_MensajeVacio_PideElAsuntoYLaDescripcion(string mensaje)
        {
            StringAssert.Contains(Responder(mensaje), "Escriba el asunto");
        }

        [TestMethod]
        [DataRow("Olvidé mi contraseña")]
        [DataRow("Necesito cambiar la clave de mi cuenta")]
        public void GetRespuesta_TemaContrasena_RemiteAAjustes(string mensaje)
        {
            StringAssert.Contains(Responder(mensaje), "Ajustes");
        }

        [TestMethod]
        [DataRow("No puedo iniciar sesión")]
        [DataRow("No me deja entrar al sistema")]
        public void GetRespuesta_TemaSesion_MencionaElBloqueoDeCuenta(string mensaje)
        {
            StringAssert.Contains(Responder(mensaje), "bloqueada");
        }

        [TestMethod]
        [DataRow("No me sumaron la XP de la partida")]
        [DataRow("Mi experiencia no subió de nivel")]
        public void GetRespuesta_TemaExperiencia_ExplicaComoSeAcredita(string mensaje)
        {
            StringAssert.Contains(Responder(mensaje), "dificultad del reto");
        }

        [TestMethod]
        public void GetRespuesta_TemaReto_ExplicaDondeSeJuega()
        {
            StringAssert.Contains(Responder("El enunciado del reto se ve cortado"), "desde Jugar");
        }

        [TestMethod]
        [DataRow("No aparezco en el marcador")]
        [DataRow("El ranking no se actualiza")]
        public void GetRespuesta_TemaMarcador_ExplicaElOrdenamiento(string mensaje)
        {
            StringAssert.Contains(Responder(mensaje), "XP acumulada");
        }

        [TestMethod]
        [DataRow("Me sale un error al guardar")]
        [DataRow("La página tiene una falla")]
        public void GetRespuesta_TemaError_PideDondeOcurrio(string mensaje)
        {
            StringAssert.Contains(Responder(mensaje), "en qué pantalla ocurrió");
        }

        [TestMethod]
        public void GetRespuesta_TemaSinPalabraClave_DevuelveLaRespuestaGenerica()
        {
            StringAssert.Contains(Responder("Quisiera felicitar al equipo"), "Un agente de soporte");
        }

        // El usuario escribe sin tildes con frecuencia; la regla tiene que responder igual.
        [TestMethod]
        public void GetRespuesta_SinTildesYEnMayusculas_CaeEnLaMismaRegla()
        {
            Assert.AreEqual(
                Responder("No puedo iniciar sesión"),
                Responder("NO PUEDO INICIAR SESION"));
        }
    }
}
