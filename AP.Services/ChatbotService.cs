using System.Globalization;
using System.Text;

namespace AP.Services
{
    /// <summary>
    /// Servicio auxiliar del chatbot de soporte (módulo Request IT).
    /// Vive en AP.Services porque no es lógica de negocio del juego
    /// ni acceso a datos: es una integración auxiliar.
    /// </summary>
    public class ChatbotService
    {
        public string GetRespuesta(string mensajeUsuario)
        {
            var texto = Normalizar(mensajeUsuario);

            if (texto.Length == 0)
                return "Escriba el asunto y la descripción del problema y le indicaremos por dónde empezar.";

            if (texto.Contains("contrasena") || texto.Contains("clave"))
                return "Para cambiar su contraseña entre a Ajustes y elija \"Cambiar su contraseña\". Si no la recuerda, un agente puede restablecerla desde la solicitud.";

            if (texto.Contains("sesion") || texto.Contains("ingresar") || texto.Contains("entrar"))
                return "Si no logra iniciar sesión, revise que el correo esté bien escrito. Después de cinco intentos fallidos la cuenta queda bloqueada cinco minutos.";

            if (texto.Contains("xp") || texto.Contains("experiencia") || texto.Contains("nivel"))
                return "La XP depende de la dificultad del reto y se acredita al terminar la partida. Si no se reflejó, indique la fecha y el modo en que jugó.";

            if (texto.Contains("reto"))
                return "Los retos se juegan desde Jugar, eligiendo un modo. Si un enunciado se ve incompleto o la respuesta correcta parece equivocada, indique el título del reto.";

            if (texto.Contains("marcador") || texto.Contains("ranking") || texto.Contains("posicion"))
                return "El marcador global ordena por XP acumulada y se actualiza al terminar cada partida. Su historial personal aparece debajo de la tabla.";

            if (texto.Contains("error") || texto.Contains("falla") || texto.Contains("pantalla"))
                return "Para revisar el error necesitamos saber en qué pantalla ocurrió y qué estaba haciendo en ese momento. Agregue esos dos datos a la descripción.";

            return "Gracias por su mensaje. Un agente de soporte revisará la solicitud y le responderá pronto.";
        }

        // Se compara sin tildes ni mayúsculas para que "sesión" y "sesion" caigan en la misma regla.
        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var limpio = new StringBuilder(descompuesto.Length);

            foreach (var caracter in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
                {
                    limpio.Append(caracter);
                }
            }

            return limpio.ToString();
        }
    }
}
