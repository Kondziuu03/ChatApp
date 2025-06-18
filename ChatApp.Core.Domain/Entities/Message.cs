namespace ChatApp.Core.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public Chat Chat { get; set; }
        public User User { get; set; }
    }
}