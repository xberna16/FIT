using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;
using TeamCommunication.DAL;
using TeamCommunication.BL.Facades;
using TeamCommunication.BL.Queries;

namespace TeamCommunication.BL
{
    public class PasswordControl : IPasswordControl
    {
        private const int SaltLengthLimit = 32;
        private readonly RNGCryptoServiceProvider _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        private readonly SHA1CryptoServiceProvider _sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();
        private readonly UserRepository _userRepository;
        private readonly ASCIIEncoding _encoding = new ASCIIEncoding();
        private readonly IDbContextFactory _dbContextFactory;

        public PasswordControl(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _userRepository = new UserRepository(dbContextFactory);
            _sha1CryptoServiceProvider.Initialize();
        }

        public UserDetailModel Registration(UserDetailModel model)
        {
            var salt = GetSalt();
            var hash = GetHashString(model.password, salt);

            model.salt = salt;
            model.password = hash;

            return _userRepository.Add(model);
        }

        public UserDetailModel Login(UserDetailModel model)
        {
            var user = _userRepository.GetByEmail(model.email);

            if (user == null)
            {
                return null;
            }

            var hash = GetHashString(model.password, user.salt);

            return hash == user.password ? user : null;
        }

        private string GetHashString(string password, string salt)
        {
            var data = Encoding.ASCII.GetBytes(String.Concat(salt, password));
            var hash = _sha1CryptoServiceProvider.ComputeHash(data);
            return _encoding.GetString(hash);
        }

        private string GetSalt()
        {
            return Convert.ToBase64String(GetSalt(SaltLengthLimit));
        }

        private byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            _rngCryptoServiceProvider.GetNonZeroBytes(salt);

            return salt;
        }
    }
}
