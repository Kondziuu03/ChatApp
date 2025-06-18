namespace ChatApp.Core.Domain.Models
{
    public class QueryResponse
    {
        public string Answer { get; set; } = string.Empty;
        public List<string> SourceChunks { get; set; } = new();
        public string FileName { get; set; } = string.Empty;
    }
}
