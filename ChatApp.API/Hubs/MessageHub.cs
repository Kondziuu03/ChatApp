using ChatApp.API.Extensions;
using ChatApp.Core.Application.Services;
using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatApp.API.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private const string MainChat = "Global";

        private readonly IChatService _chatService;
        private readonly UserConnectionService _userConnectionService;

        public MessageHub(IChatService chatService, UserConnectionService userConnectionService)
        {
            _chatService = chatService;
            _userConnectionService = userConnectionService;
        }

        public override async Task OnConnectedAsync()
        {
            _userConnectionService.AddConnection(GetUsername(), Context.ConnectionId);

            await JoinChat(MainChat);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userConnectionService.RemoveConnection(GetUsername());

            await LeaveChat(MainChat);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToChat(string chatId, string message)
        {
            await Clients.Group(MainChat).SendAsync("ReceiveMessage", GetUsername(), message);

            await _chatService.SaveMessageAsync(CreateMessageDto(chatId, message));
        }

        private async Task LeaveChat(string chatName) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);

        private async Task JoinChat(string chatName) =>
           await Groups.AddToGroupAsync(Context.ConnectionId, chatName);

        private string GetUsername() =>
            Context.User.GetClaimValue(ClaimTypes.NameIdentifier);

        private string GetUserId() =>
            Context.User.GetClaimValue(JwtRegisteredClaimNames.Jti);

        private MessageDto CreateMessageDto(string chatId, string message) =>
           new()
           {
               Text = message,
               ChatId = Guid.Parse(chatId),
               Username = GetUsername(),
               UserId = Guid.Parse(GetUserId())
           };
    }
}
