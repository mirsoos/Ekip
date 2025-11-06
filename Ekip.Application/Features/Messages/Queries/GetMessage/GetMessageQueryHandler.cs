using Ekip.Application.DTOs;
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
            var messages = await _readMessage.GetMessagesAsync(request.ChatRoomId, request.take, cancellationToken);

            var messageDtos = messages.Select(s => new MessageDto
            {
                Id = s.Id,
                ChatRoomId = s.Chatroom.Id,
                MessageContent = s.MessageContent,
                SenderId = s.SenderId,
                SentAt = s.SentAt,
                IsDeleted = s.IsDeleted,
                IsEdited = s.IsEdited,
                Type = s.Type

            }).OrderBy(o=>o.SentAt).ToList();

            return messageDtos;
        }
    }
}
