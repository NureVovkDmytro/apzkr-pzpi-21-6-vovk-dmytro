namespace Discerniy.Domain.Entity.Options
{
    public class AuthServiceOption
    {
        public int SaltByteSize { get; set; }
        public int HashByteSize { get; set; }
        public int HasingIterationsCount { get; set; }
        public int ConfirmationCodeByteSize { get; set; }
        public int SecretKeyCharacterCount { get; set; }
    }
}
