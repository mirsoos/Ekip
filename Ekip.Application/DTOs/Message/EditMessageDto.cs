
namespace Ekip.Application.DTOs.Chat
{
    public class EditMessageDto
    {
        public Guid SenderRef { get; set; }
        public string MessageContent { get; set; }
        public Guid MessageRef { get; set; }
    }
}
