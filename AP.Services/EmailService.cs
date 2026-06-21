namespace AP.Services
{
    /// <summary>
    /// Servicio auxiliar de correo (recuperación de contraseña, avisos del foro).
    /// Implementación pendiente: SmtpClient o el IIdentityMessageService de Identity.
    /// </summary>
    public class EmailService
    {
        public void Send(string to, string subject, string body)
        {
            // TODO: implementar con System.Net.Mail.SmtpClient
            // y credenciales en Web.config (nunca en código).
        }
    }
}
