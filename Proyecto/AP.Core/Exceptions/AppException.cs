using System;
using System.Runtime.Serialization;

namespace AP.Core.Exceptions
{
    /// <summary>
    /// Excepción de DOMINIO de MathemaX: representa violaciones de reglas de
    /// negocio (ej. "respuesta fuera de tiempo", "reto inactivo"), no fallas
    /// técnicas. El BaseController de AP.MVC la captura y la trata distinto
    /// de una Exception inesperada (advertencia vs error).
    /// </summary>
    [Serializable]
    public class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message)
            : base(message)
        {
        }

        public AppException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
