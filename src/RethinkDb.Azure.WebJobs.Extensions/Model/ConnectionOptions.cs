using System;

namespace RethinkDb.Azure.WebJobs.Extensions.Model
{
    internal struct ConnectionOptions : IEquatable<ConnectionOptions>
    {
        #region Properties
        public string Hostname { get; private set; }

        public int? Port { get; private set; }
        #endregion

        #region Constructor
        public ConnectionOptions(string hostname, int? port)
            : this()
        {
            Hostname = hostname ?? throw new ArgumentNullException(nameof(hostname));
            Port = port;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is ConnectionOptions connectionOptions))
            {
                return false;
            }

            return Equals(connectionOptions);
        }

        public bool Equals(ConnectionOptions other)
        {
            return ((Hostname, Port) == (other.Hostname, other.Port));
        }

        public override int GetHashCode()
        {
            return (Hostname, Port).GetHashCode();
        }
        #endregion
    }
}
