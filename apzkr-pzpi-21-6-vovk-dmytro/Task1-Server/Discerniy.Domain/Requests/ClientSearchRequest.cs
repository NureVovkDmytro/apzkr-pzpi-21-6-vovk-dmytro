using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Requests
{
    public class ClientSearchRequest : PageRequest
    {
        public string? Nickname { get; set; }
        public int? AccessLevel { get; set; }
        public string? GroupId { get; set; }
        public ClientStatus? Status { get; set; } = null;
    }
}
