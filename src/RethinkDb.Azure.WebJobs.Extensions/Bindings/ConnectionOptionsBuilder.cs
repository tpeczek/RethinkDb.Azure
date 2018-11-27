using System;
using Microsoft.Azure.WebJobs;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Bindings
{
    internal static class ConnectionOptionsBuilder
    {
        public static ConnectionOptions Build(RethinkDbAttribute attribute, RethinkDbOptions options)
        {
            return new ConnectionOptions(
                attribute.HostnameSetting ?? options.Hostname,
                (String.IsNullOrEmpty(attribute.PortSetting) || !Int32.TryParse(attribute.PortSetting, out int port)) ? options.Port : port,
                attribute.AuthorizationKeySetting ?? options.AuthorizationKey,
                attribute.UserSetting ?? options.User,
                attribute.PasswordSetting ?? options.Password,
                (String.IsNullOrEmpty(attribute.EnableSslSetting) || !Boolean.TryParse(attribute.EnableSslSetting, out bool enableSsl)) ? options.EnableSsl : enableSsl,
                attribute.LicenseToSetting ?? options.LicenseTo,
                attribute.LicenseKeySetting ?? options.LicenseKey
            );
        }
    }
}
