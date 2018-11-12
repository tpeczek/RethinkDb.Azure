using System;

namespace RethinkDb.Azure.WebJobs.Extensions.Model
{
    internal class ConnectionOptions : IEquatable<ConnectionOptions>
    {
        #region Fields
        private int? _hashCode;
        #endregion

        #region Properties
        public string Hostname { get; }

        public int? Port { get; }

        public string AuthorizationKey { get; }

        public string User { get; }

        public string Password { get; }
        #endregion

        #region Constructor
        public ConnectionOptions(string hostname, int? port, string authorizationKey, string user, string pasword)
        {
            Hostname = hostname ?? throw new ArgumentNullException(nameof(hostname));
            Port = port;
            AuthorizationKey = authorizationKey;
            User = user;
            Password = pasword;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectionOptions);
        }

        public bool Equals(ConnectionOptions other)
        {
            if (other is null)
            {
                return false;
            }

            return (Hostname == other.Hostname)
                && (Port == other.Port)
                && (AuthorizationKey == other.AuthorizationKey)
                && (User == other.User)
                && (Password == other.Password);
        }

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                _hashCode = (Hostname, Port, AuthorizationKey, User, Password).GetHashCode();
            }

            return _hashCode.Value;
        }
        #endregion

        #region Operators
        public static bool operator ==(ConnectionOptions left, ConnectionOptions right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ConnectionOptions left, ConnectionOptions right)
        {
            return !(left == right);
        }
        #endregion
    }
}
