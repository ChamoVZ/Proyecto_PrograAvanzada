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
            // TODO: lógica de respuestas (basada en reglas o API externa).
            return "Gracias por tu mensaje. Un agente de soporte lo revisará pronto.";
        }
    }
}
