using Microsoft.AspNetCore.Mvc;
using Utils.Messaging;

namespace WebApiProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class SendMessageController(ILogger<SendMessageController> logger, MessageSender messageSender)
    : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return messageSender.SendMessage();
    }
}
