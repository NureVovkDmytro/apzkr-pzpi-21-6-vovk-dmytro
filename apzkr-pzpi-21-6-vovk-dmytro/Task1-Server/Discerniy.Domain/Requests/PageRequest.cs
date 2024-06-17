using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Requests
{
    public class PageRequest
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; }
        [Range(1, int.MaxValue)]
        public int Limit { get; set; }
    }
}
