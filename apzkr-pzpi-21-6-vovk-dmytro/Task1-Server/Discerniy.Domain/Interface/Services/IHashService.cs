namespace Discerniy.Domain.Interface.Services
{
    public interface IHashService
    {
        string Hash(string data);
        bool Verify(string data, string hash);
    }
}
