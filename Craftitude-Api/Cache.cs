using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using YaTools.Yaml;
using Raven;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Storage;
using Raven.Database;

namespace Craftitude
{
    public class Cache
    {
        // TODO: Add status event(s)

        internal string _path;
        internal Client _client;

        internal Cache(Client client, string folderPath)
        {
            this._client = client;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            this._path = Path.GetFullPath(folderPath);
        }

        public RepositoryCache GetRepositoryCache(string cacheId)
        {
            return new RepositoryCache(this, cacheId);
        }

        public RepositoryCache GetRepositoryCache(Uri url)
        {
            return GetRepositoryCache(GenerateCacheId(url.AbsoluteUri));
        }

        private static string GenerateCacheId(string url)
        {
            return BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", "\\");
        }
    }
}
