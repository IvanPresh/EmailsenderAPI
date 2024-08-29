using EmailsenderAPI.DTO;
using MailKit.Net.Smtp;
using MimeKit;


namespace EmailsenderAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(EmailDTO emailDTO) 
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(emailDTO.From));

            email.To.Add(MailboxAddress.Parse(emailDTO.To));

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDTO.Body };
           
            using var smtp = new SmtpClient();
            
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, 
                MailKit.Security.SecureSocketOptions.Auto,CancellationToken.None);
            smtp.Authenticate(_configuration.GetSection("EmailUserName")
                .Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);

            smtp.Disconnect(true);
        }
    }
}
