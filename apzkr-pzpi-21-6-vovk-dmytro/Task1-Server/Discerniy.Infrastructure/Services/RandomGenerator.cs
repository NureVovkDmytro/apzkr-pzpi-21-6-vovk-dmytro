using Discerniy.Domain.Interface.Services;

namespace Discerniy.Infrastructure.Services
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random random = new Random();

        public string GenerateString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
