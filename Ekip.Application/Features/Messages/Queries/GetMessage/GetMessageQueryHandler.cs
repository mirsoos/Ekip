using Ekip.Application.DTOs.Chat;
using Ekip.Application.Interfaces;
using MediatR;


namespace Ekip.Application.Features.Messages.Queries.GetMessage
{
    public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery,List<MessageDto>>
    {
        private readonly IMessageReadRepository _readMessage;
        public GetMessageQueryHandler(IMessageReadRepository readMessage)
        {
            _readMessage = readMessage;
        }

        public async Task<List<MessageDto>> Handle(GetMessageQuery request,CancellationToken cancellationToken)
        {
            
            var messages = await _readMessage.GetMessagesAsync(request.ChatRoomRef,request.Take,cancellationToken);

            var messageDtos = messages.Select(s => new MessageDto
            {
                Id = s.Id,
                ChatRoomRef = s.ChatRoomRef,
                MessageContent = s.MessageContent,
                SenderRef = s.SenderRef,
                SentAt = s.SentAt,
                IsDeleted = s.IsDeleted,
                IsEdited = s.IsEdited,
                Type = s.Type

            }).OrderBy(o=>o.SentAt).ToList();

            return messageDtos;
        }
    }
}
