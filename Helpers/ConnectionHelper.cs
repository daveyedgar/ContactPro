﻿

using Npgsql;

namespace ContactPro.Helpers
{
    public static class ConnectionHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("pgSettings")["pgConnection"];

            //RAILWAY
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        // build connection string from environment for Raleway
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        //private static string BuildConnectionString(string databaseurl)
        //{
        //    var databaseuri = new Uri(databaseurl);
        //    var userInfo = databaseuri.UserInfo.Split(':');

        //    return new NpgsqlConnectionStringBuilder()
        //    {
        //        Host = databaseuri.Host,
        //        Port = databaseuri.Port,
        //        Username = userInfo[0],
        //        Password = userInfo[1],
        //        Database = databaseuri.LocalPath.TrimStart('/'),
        //        SslMode = SslMode.Prefer,
        //        TrustServerCertificate = true
        //    }.ToString();
        //}
    }
}
