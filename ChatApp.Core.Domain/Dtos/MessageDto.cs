namespace ChatApp.Core.Domain.Dtos
{
    public class MessageDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid ChatId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}