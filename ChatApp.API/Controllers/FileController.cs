using ChatApp.Core.Domain.Exceptions;
using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IPdfProcessingService _pdfProcessingService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IRagService _ragService;

        public FileController(ILogger<FileController> logger, IPdfProcessingService pdfProcessingService, IEmbeddingService embeddingService, IRagService ragService)
        {
            _logger = logger;
            _pdfProcessingService = pdfProcessingService;
            _embeddingService = embeddingService;
            _ragService = ragService;
        }

        [HttpPost("Upload")]
        public IActionResult UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("No file provided");

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Only PDF files are supported");

            _logger.LogInformation($"Processing PDF: {file.FileName}");

            var chunks = _pdfProcessingService.ProcessPdf(file);

            _logger.LogInformation($"Successfully processed {chunks.Count} chunks from {file.FileName}");

            return Ok(new
            {
                message = "PDF processed successfully",
                filename = file.FileName,
                chunks = chunks
            });
        }

        [HttpPost("Test")]
        public async Task<IActionResult> Test(string text)
        {
            //var result = await _embeddingService.GenerateEmbeddingAsync(text);
            await _ragService.StoreDocumentChunksAsync(new List<DocumentChunk> { new DocumentChunk { ChunkIndex = 1, FileName = "test", Content = "test33 test455" } });
            return Ok();
        }
    }
}
