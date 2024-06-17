namespace Discerniy.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException() : base("Bad request")
        {
        }
    }
}
