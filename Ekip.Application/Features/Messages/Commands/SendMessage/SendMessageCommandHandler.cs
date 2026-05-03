using Ekip.Application.Contracts.Events;
using Ekip.Application.DTOs.Chat;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Chat.Entites;
using MediatR;

namespace Ekip.Application.Features.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand,MessageDto>
    {
        private readonly IMessageWriteRepository _messageWrite;
        private readonly IChatRoomWriteRepository _chatRoomWrite;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;
        public SendMessageCommandHandler(IMessageWriteRepository writeMessage,IChatRoomWriteRepository chatRoomWrite , IEventPublisher eventPublisher , IUnitOfWork unitOfWork) 
        {
            _messageWrite = writeMessage;
            _chatRoomWrite = chatRoomWrite;
            _eventPublisher = eventPublisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request ,CancellationToken cancellationToken)
        {
            var chatRoom = await _chatRoomWrite.GetByIdAsync(request.ChatRoomRef, cancellationToken);

            if (chatRoom == null)
                throw new Exception("ChatRoom Not Found.");

            var message = chatRoom.CreateMessage(request.SenderRef , request.MessageContent , request.ReplyToMessageRef);
            Message savedMessage = null!;
            await _unitOfWork.ExecuteAsync(async (innerCt) =>
            {
                savedMessage = await _messageWrite.AddAsync(message, innerCt);

                await _eventPublisher.Publish(new MessageCreatedEvent
                {
                    Id = savedMessage.Id,
                    SenderRef = savedMessage.SenderRef,
                    ChatRoomRef = savedMessage.ChatRoomRef,
                    MessageContent = savedMessage.MessageContent,
                    SentAt = savedMessage.SentAt,
                    Type = savedMessage.Type,
                    ReplyToMessageRef = savedMessage.ReplyToMessageRef,
                }, innerCt);

            },cancellationToken);

            return new MessageDto
            {
                Id = savedMessage.Id,
                IsDeleted = savedMessage.IsDeleted,
                IsEdited = savedMessage.IsEdited,
                MessageContent = savedMessage.MessageContent,
                SenderRef = savedMessage.SenderRef,
                SentAt = savedMessage.SentAt,
                Type = savedMessage.Type,
                ChatRoomRef = savedMessage.ChatRoomRef
                
            };

        }
    }
}
