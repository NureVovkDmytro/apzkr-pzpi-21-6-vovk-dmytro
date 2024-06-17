using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Requests
{
    public class SearchGroupMemberRequest : UsersSearchRequest
    {
        public ClientType? Type { get; set; }


        public ClientType? SearchType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName) || !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(TaxPayerId) || Type == ClientType.User)
                {
                    return ClientType.User;
                }
                return Type;
            }
        }
    }
}
