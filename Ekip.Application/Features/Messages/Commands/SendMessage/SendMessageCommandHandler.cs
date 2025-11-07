using Ekip.Application.DTOs;
using Ekip.Application.Interfaces;
using MediatR;


namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand,MessageDto>
    {
        private readonly IMessageWriteRepository _writeMessage;
        private readonly IChatRoomReadRepository _chatRoomRepository;
        public SendMessageCommandHandler(IMessageWriteRepository writeMessage,IChatRoomReadRepository chatRoomRepository) 
            {
                _writeMessage = writeMessage;
                _chatRoomRepository = chatRoomRepository;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request ,CancellationToken cancellationToken)
        {

            var chatRoom = await _chatRoomRepository.GetByIdAsync(request.ChatRoomId, cancellationToken);

            if (chatRoom == null)
                throw new Exception("ChatRoom Not Found");

            var message = chatRoom.SendMessage(request.SenderId,request.MessageContent);

            await _writeMessage.AddAsync(message, cancellationToken);

            return new MessageDto
            {
                Id = message.Id,
                IsDeleted = message.IsDeleted,
                IsEdited = message.IsEdited,
                MessageContent = message.MessageContent,
                SenderId = message.SenderId,
                SentAt = message.SentAt,
                Type = message.Type,
                ChatRoomId = message.ChatRoom.Id

            };

        }
    }
}
