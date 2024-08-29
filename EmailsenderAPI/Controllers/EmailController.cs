using EmailsenderAPI.DTO;
using EmailsenderAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailsenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendEmail(EmailDTO emailDTO) 
        { 
            _emailService.SendEmail(emailDTO);  

            return Ok($"please check {emailDTO.To}");
        
        }
    }
}
