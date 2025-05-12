namespace ChatApp.Core.Domain.Dtos
{
    public class AuthDto
    {
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
