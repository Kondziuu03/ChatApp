namespace ChatApp.Core.Domain.Models
{
    public class DocumentChunk
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
