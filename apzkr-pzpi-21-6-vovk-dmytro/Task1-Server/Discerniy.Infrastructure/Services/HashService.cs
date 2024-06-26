﻿using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Services;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Discerniy.Infrastructure.Services
{
    public class HashService : IHashService
    {
        protected readonly AuthServiceOption authConfig;

        public HashService(IOptions<AppConfig> appConfig)
        {
            this.authConfig = appConfig.Value.AuthService;
        }

        public string Hash(string password)
        {
            byte[] salt;

            using (var generator = RandomNumberGenerator.Create())
            {
                salt = new byte[authConfig.SaltByteSize];
                generator.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, authConfig.HasingIterationsCount, HashAlgorithmName.SHA512);
            byte[] hash = pbkdf2.GetBytes(authConfig.HashByteSize);
            byte[] hashBytes = new byte[authConfig.HashByteSize + authConfig.SaltByteSize];

            Array.Copy(salt, 0, hashBytes, 0, authConfig.SaltByteSize);
            Array.Copy(hash, 0, hashBytes, authConfig.SaltByteSize, authConfig.HashByteSize);

            return Convert.ToBase64String(hashBytes);
        }

        public bool Verify(string password, string passwordHash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(passwordHash);

                byte[] salt = new byte[authConfig.SaltByteSize];
                Array.Copy(hashBytes, 0, salt, 0, authConfig.SaltByteSize);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, authConfig.HasingIterationsCount, HashAlgorithmName.SHA512);
                byte[] hash = pbkdf2.GetBytes(authConfig.HashByteSize);

                for (int i = 0; i < authConfig.HashByteSize; i++)
                    if (hashBytes[i + authConfig.SaltByteSize] != hash[i])
                        return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
