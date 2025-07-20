using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace ChatApp.Core.Application.Services
{
    public class PdfProcessingService : IPdfProcessingService
    {
        private const int ChunkSize = 1000;
        private const int ChunkOverlap = 200;

        private readonly ILogger<PdfProcessingService> _logger;

        public PdfProcessingService(ILogger<PdfProcessingService> logger)
        {
            _logger = logger;
        }

        public List<DocumentChunk> ProcessPdf(IFormFile file)
        {
            var chunks = new List<DocumentChunk>();

            using var stream = file.OpenReadStream();
            using var document = PdfDocument.Open(stream);

            var fullText = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                fullText.AppendLine(page.Text);

                _logger.LogDebug($"Extracted {page.Text.Length} characters from page {page.Number}");
            }

            var textChunks = CreateChunks(fullText.ToString(), ChunkSize, ChunkOverlap);

            for (int i = 0; i < textChunks.Count; i++)
            {
                chunks.Add(new DocumentChunk
                {
                    Text = textChunks[i],
                    FileName = file.FileName,
                    ChunkIndex = i
                });
            }

            return chunks;
        }

        private List<string> CreateChunks(string text, int chunkSize, int overlap)
        {
            var chunks = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("PDF contains no readable text.");
                return chunks;
            }

            var sentences = Regex.Split(text, @"(?<=[.!?])\s+");

            var currentChunk = new StringBuilder();
            var currentSize = 0;

            foreach (var sentence in sentences)
            {
                var trimmedSentence = sentence.Trim();
                if (string.IsNullOrEmpty(trimmedSentence)) continue;

                if (currentSize + trimmedSentence.Length > chunkSize && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString().Trim());

                    var overlapText = GetLastCharacters(currentChunk.ToString(), overlap);
                    currentChunk.Clear();
                    currentChunk.Append(overlapText);
                    currentSize = overlapText.Length;
                }

                currentChunk.Append(trimmedSentence + ". ");
                currentSize += trimmedSentence.Length + 2;
            }

            if (currentChunk.Length > 0)
                chunks.Add(currentChunk.ToString().Trim());

            return chunks;
        }

        private string GetLastCharacters(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Length <= maxLength
                ? text
                : text.Substring(text.Length - maxLength);
        }

    }
}
