using ChatApp.Core.Domain.Exceptions;
using ChatApp.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IPdfProcessingService _pdfProcessingService;
        private readonly IRagService _ragService;

        public FileController(ILogger<FileController> logger, IPdfProcessingService pdfProcessingService, IRagService ragService)
        {
            _logger = logger;
            _pdfProcessingService = pdfProcessingService;
            _ragService = ragService;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("No file provided");

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Only PDF files are supported");

            _logger.LogInformation($"Processing PDF: {file.FileName}");

            var chunks = _pdfProcessingService.ProcessPdf(file);

            await _ragService.StoreDocumentChunksAsync(chunks);

            _logger.LogInformation($"Successfully processed {chunks.Count} chunks from {file.FileName}");

            return Ok(new
            {
                message = "PDF processed successfully",
                filename = file.FileName
            });
        }
    }
}
