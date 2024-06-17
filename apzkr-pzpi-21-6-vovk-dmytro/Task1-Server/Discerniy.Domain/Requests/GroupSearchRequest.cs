namespace Discerniy.Domain.Requests
{
    public class GroupSearchRequest : PageRequest
    {
        public string? Name { get; set; }
        public int? AccessLevel { get; set; }
    }
}
