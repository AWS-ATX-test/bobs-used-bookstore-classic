using System;
using System.Collections.Generic;

namespace BobsBookstoreClassic.Data
{
    public sealed class BookstoreConfiguration
    {
        private static readonly Lazy<BookstoreConfiguration> Lazy = new Lazy<BookstoreConfiguration>(() => new BookstoreConfiguration());

        private static BookstoreConfiguration Instance => Lazy.Value;

        private readonly Dictionary<string, string> _appSettings = new Dictionary<string, string>();

        private BookstoreConfiguration()
        {
            // Configuration is no longer read from ConfigurationManager.AppSettings.
            // The host is responsible for populating settings via the Add() method
            // or through environment variables retrieved at access time.
        }

        public static void Add(string key, string value)
        {
            Instance._appSettings[key] = value;
        }

        public static string Get(string key)
        {
            // Check environment variable override first, then fall back to stored settings
            var envValue = Environment.GetEnvironmentVariable(key);
            if (envValue != null)
            {
                return envValue;
            }

            return Instance._appSettings.ContainsKey(key) ? Instance._appSettings[key] : null;
        }

        public static T Get<T>(string key)
        {
            var value = Get(key);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
