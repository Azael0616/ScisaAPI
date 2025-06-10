using Microsoft.Extensions.Options;
using ScisaAPI.Models;
using ScisaAPI.Utils.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ScisaAPI.Utils
{
    public class ServicioCorreo: IServicioCorreo
    {
        private readonly SmtpConfiguracion _smtpConfiguracion;
        
        public ServicioCorreo(IOptions<SmtpConfiguracion> smtpOpciones)
        {
            _smtpConfiguracion = smtpOpciones.Value;
        }
        public async Task EnviarCorreo(List<Pokemon> lista)
        {
            var mensaje = new MailMessage();
            mensaje.From = new MailAddress("pruebasalr16@gmail.com", "ScisaAPI");
            mensaje.To.Add("sect1965@hotmail.com");
            mensaje.Subject = "Listado Pokemon";
            mensaje.IsBodyHtml = true;

            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append("<h2>Listado Pokémon</h2><ul>");
            foreach (var item in lista)
            {
                bodyBuilder.Append($"<li>ID: {item.Id}, Nombre: {item.Nombre} <br/><img src='{item.Imagen}' width='64px' height='64px' /></li>");
            }
            bodyBuilder.Append("</ul>");

            mensaje.Body = bodyBuilder.ToString();

            using var smtp = new SmtpClient(_smtpConfiguracion.Host, _smtpConfiguracion.Port)
            {
                Credentials = new NetworkCredential(_smtpConfiguracion.User, _smtpConfiguracion.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mensaje);            
        }
    }
}
