namespace ChatApp.Core.Domain.Models
{
    public class DocumentChunk
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
    }
}
