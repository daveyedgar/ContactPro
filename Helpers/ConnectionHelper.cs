﻿using Npgsql;

namespace ContactPro.Helpers
{
    public class ConnectionHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("pgSettings")["pgConnection"];

            //HEROKU
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            //RAILWAY


            return (string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl));
        }

        // build connection string from environment for Raleway
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var buidler = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return buidler.ToString();
        }
    }
}