using EmailsenderAPI.DTO;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Serilog;

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
            try
            {
                // Create a new MimeMessage object with the email details
                var email = new MimeMessage
                {
                    From = { MailboxAddress.Parse(emailDTO.From) }, // Sender's email address
                    To = { MailboxAddress.Parse(emailDTO.To) }, // Recipient's email address
                    Subject = emailDTO.Subject, // Subject of the email
                    Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDTO.Body } // Email body in HTML format
                };

                using var smtp = new SmtpClient(); // Create an instance of SmtpClient
                bool connected = false;

                try
                {
                    // Try connecting with StartTls on port 587 first
                    smtp.Connect(_configuration["EmailHost"], 587, MailKit.Security.SecureSocketOptions.StartTls);
                    connected = true;
                }
                catch (SmtpCommandException ex)
                {
                    // Log warning and attempt to connect on a different port if connection fails
                    Log.Warning(ex, "Failed to connect on port 587. Attempting port 465.");
                }

                if (!connected)
                {
                    try
                    {
                        // Try connecting with SSL on port 465
                        smtp.Connect(_configuration["EmailHost"], 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                        connected = true; // Set connected to true if connection is successful
                    }
                    catch (SmtpCommandException innerEx)
                    {
                        // Log error and throw an exception if connection fails on both ports
                        Log.Error(innerEx, "Failed to connect on port 465.");
                        throw new InvalidOperationException("Failed to connect to SMTP server on both ports 587 and 465.", innerEx);
                    }
                }

                if (!connected)
                {
                    // Throw an exception if unable to connect to the SMTP server
                    throw new InvalidOperationException("Unable to connect to SMTP server.");
                }

                // Authenticate with the SMTP server using the provided username and password
                smtp.Authenticate(_configuration["EmailUserName"], _configuration["EmailPassword"]);

                smtp.Send(email);

                // Disconnect from the SMTP server
                smtp.Disconnect(true);
            }
            catch (SmtpCommandException ex)
            {
                // Log SMTP-specific errors
                Log.Error(ex, "SMTP Command Error: {Message}", ex.Message);
                throw new InvalidOperationException($"SMTP Command Error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Log any other general errors
                Log.Error(ex, "Failed to send email: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to send email", ex);
            }
        }
    }
}
