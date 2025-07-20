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
        private readonly IRagService _ragService;

        public ChatController(IChatService chatService, IRagService ragService)
        {
            _chatService = chatService;
            _ragService = ragService;
        }

        [HttpGet("GetPaginatedChat")]
        public async Task<IActionResult> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
        {
            var chat = await _chatService.GetPaginatedChatAsync(chatName, pageNumber, pageSize);

            return Ok(chat);
        }

        [HttpGet("ParaphraseMessage")]
        public async Task<IActionResult> ParaphraseMessage(string message, string style)
        {
            if (string.IsNullOrEmpty(message))
                throw new BadRequestException("Text to paraphrase cannot be empty");

            var result = await _chatService.ParaphraseMessageAsync(message, style);

            return Ok(new { message = result });
        }

        [HttpGet("CheckGrammar")]
        public async Task<IActionResult> CheckGrammar(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new BadRequestException("Text to check cannot be empty");

            var result = await _chatService.CheckGrammarAsync(message);

            return Ok(new { message = result });
        }

        [HttpGet("GenerateResponse")]
        public async Task<IActionResult> GenerateResponse(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new BadRequestException("Message cannot be empty");

            var result = await _ragService.QueryAsync(message);

            return Ok(new { result });
        }
    }
}
