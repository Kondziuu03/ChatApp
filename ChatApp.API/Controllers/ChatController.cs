using ChatApp.Core.Domain.Exceptions;
using ChatApp.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("GetPaginatedChat")]
        public async Task<IActionResult> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
        {
            var chat = await _chatService.GetPaginatedChat(chatName, pageNumber, pageSize);

            return Json(chat);
        }

        [HttpPost("ParaphraseMessage")]
        public async Task<IActionResult> ParaphraseMessage(string message, string style)
        {
            if (string.IsNullOrEmpty(message))
                throw new BadRequestException("Text to paraphrase cannot be empty");

            var result = await _chatService.ParaphraseMessage(message, style);

            return Json(new { message = result });
        }

        [HttpPost("CheckGrammar")]
        public async Task<IActionResult> CheckGrammar(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new BadRequestException("Text to check cannot be empty");

            var result = await _chatService.CheckGrammar(message);

            return Json(new { message = result });
        }
    }
}
