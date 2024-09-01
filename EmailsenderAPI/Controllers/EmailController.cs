using EmailsenderAPI.DTO; // Using directive for the Data Transfer Object (DTO) namespace
using EmailsenderAPI.Services; // Using directive for the Email Service namespace
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailsenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService; // Dependency injection of EmailService
        private readonly ILogger<EmailController> _logger; // Dependency injection of ILogger

        // Constructor to initialize the email service and logger
        public EmailController(EmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        // POST: api/Email
        [HttpPost]
        public IActionResult SendEmail([FromBody] EmailDTO emailDTO) // Expecting the EmailDTO to be passed in the request body
        {
            try
            {
                // Call the email service to send the email
                _emailService.SendEmail(emailDTO);

                // Return an OK response with the email recipient information
                return Ok($"Please check {emailDTO.To}");
            }
            catch (Exception ex)
            {
                // Log the error with the provided exception and message
                _logger.LogError(ex, "An error occurred while sending the email: {Message}", ex.Message);

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while sending the email.");
            }
        }
    }
}
