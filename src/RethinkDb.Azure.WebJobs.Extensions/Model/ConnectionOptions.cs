using System;

namespace RethinkDb.Azure.WebJobs.Extensions.Model
{
    internal struct ConnectionOptions
    {
        #region Properties
        public string Hostname { get; private set; }
        #endregion

        #region Constructor
        public ConnectionOptions(string hostname)
            : this()
        {
            Hostname = hostname ?? throw new ArgumentNullException(nameof(hostname));
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is ConnectionOptions connectionOptions))
            {
                return false;
            }

            return (connectionOptions.Hostname == Hostname);
        }

        public override int GetHashCode()
        {
            return Hostname.GetHashCode();
        }
        #endregion
    }
}
