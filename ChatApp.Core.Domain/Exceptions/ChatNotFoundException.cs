namespace ChatApp.Core.Domain.Exceptions
{
    public class ChatNotFoundException : Exception
    {
        public ChatNotFoundException(string chatName)
            : base($"Chat with name: {chatName} does not exist") {}
    }
}
