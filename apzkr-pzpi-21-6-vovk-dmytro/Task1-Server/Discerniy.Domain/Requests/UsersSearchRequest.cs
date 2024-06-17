namespace Discerniy.Domain.Requests
{
    public class UsersSearchRequest : ClientSearchRequest
    {
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? TaxPayerId { get; set; } = null;
    }
}
