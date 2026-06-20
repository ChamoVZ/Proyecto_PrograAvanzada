using System.Linq;

namespace AP.Core.Validations
{
    /// <summary>
    /// Validaciones reutilizables en toda la solución (mismo rol que en el
    /// proyecto de referencia). Métodos estáticos puros: fáciles de probar.
    /// </summary>
    public static class GlobalValidation
    {
        /// <summary>Longitud mínima exigida por la política del curso.</summary>
        public const int MinPasswordLength = 12;

        public static bool ValidatePassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= MinPasswordLength;
        }

        public static bool ValidatePassword(string password, bool checkCaps)
        {
            return ValidatePassword(password) && (!checkCaps || password.Any(char.IsUpper));
        }
    }
}
