using SendGrid;
using SendGrid.Helpers.Mail;

namespace TFG.Services
{

    public interface IMailService
    {
        public Task SendEmailAsync(string destinatario, string nombredest, string urlrec);
    }
    public class MailService : IMailService
    {
        private readonly IConfiguration configuration;
        public MailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string destinatario, string nombredest, string codigo)
        {
            var apikey = configuration["sendgridapikey"];
            var remitente = "jujise96@outlook.com";
            var idtemplate = "d-16b4c51af57943e5984d549799e0d021";
            var cliente = new SendGridClient(apikey);
            var from = new EmailAddress(remitente, "TFGGSY");
            var to = new EmailAddress(destinatario, nombredest);
            var cuerpomensaje = new SendGridMessage();
            cuerpomensaje.SetFrom(from);
            cuerpomensaje.AddTo(to);
            cuerpomensaje.SetTemplateId(idtemplate);
            cuerpomensaje.SetTemplateData(new
            {
                codigo = codigo
            });
            var response = await cliente.SendEmailAsync(cuerpomensaje);
        }
    }
}
