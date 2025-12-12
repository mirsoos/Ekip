
namespace Ekip.Application.DTOs.Chat
{
    public class EditMessageDto
    {
        public long SenderRef { get; set; }
        public string MessageContent { get; set; }
        public long MessageRef { get; set; }
    }
}
