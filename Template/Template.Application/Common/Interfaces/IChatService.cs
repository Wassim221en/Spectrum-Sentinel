using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Common.Interfaces;

public interface IChatService
{
    Task<OperationResponse<Guid>>CreateChat(Guid userAId, Guid userBId);
    Task<OperationResponse>SendMessage(Guid chatId, string message, Guid senderId);
}