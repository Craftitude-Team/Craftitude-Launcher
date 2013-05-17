using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using YaTools.Yaml;
using Raven;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Linq;
using Raven.Database;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json;
using Raven.Json.Linq;
using Raven.Storage;

namespace Craftitude
{
    public class Client
    {
        public string BasePath { get; private set; }
        public Cache Cache { get; private set; }
        public List<DocumentStore> Repositories { get; private set; }
        public ObservableCollection<Operation> Operations { get; private set; } // TODO: Make operations list read-only to public
        private int nextOperationIndexToAssign = 0;

        protected static Uri GetHttpFromCrepUrl(Uri repositoryUri)
        {
            if (!repositoryUri.Scheme.Equals("crep", StringComparison.OrdinalIgnoreCase))
                throw new UriFormatException("Not a valid craftitude repository URI.");

            string database = repositoryUri.AbsolutePath.Split('/').Last();
            if (string.IsNullOrEmpty(database))
                throw new UriFormatException("Not a valid Craftitude repository URI.");

            var uriRebuilder = new UriBuilder(repositoryUri);
            uriRebuilder.Path = repositoryUri.AbsolutePath.Substring(0, repositoryUri.AbsolutePath.Length - database.Length - 1);
            uriRebuilder.Scheme = "http";

            return uriRebuilder.Uri;
        }

        public void AddRepository(string repositoryUrl)
        {
            AddRepository(new Uri(repositoryUrl));
        }

        public void AddRepository(Uri repositoryUri)
        {
            repositoryUri = GetHttpFromCrepUrl(repositoryUri);

            if (Repositories.Where(r => r.Url == repositoryUri.ToString()).Any())
                return; // already added

            var document = new DocumentStore();
            document.Url = repositoryUri.ToString();
            document.DefaultDatabase = "";
            document.Initialize();

            Repositories.Add(document);
        }

        public void DeleteRepository(Uri repositoryUri)
        {
            repositoryUri = GetHttpFromCrepUrl(repositoryUri);
            Repositories.RemoveAll(r => r.Url == repositoryUri.ToString());
        }

        public Client(string basePath)
        {
            if(!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            this.BasePath = Path.GetFullPath(basePath);

            string craftitudeBasePath = Path.Combine(basePath, ".craftitude");
            if(!Directory.Exists(craftitudeBasePath))
                Directory.CreateDirectory(craftitudeBasePath);

            this.Cache = new Cache(this, Path.Combine(craftitudeBasePath, "craftitude.cache"));

            // Load repositories
            this.Repositories = new List<Repository>();
            string repositoriesConf = Path.Combine(craftitudeBasePath, "repositories.conf");
            if (!File.Exists(repositoriesConf))
            {
                using (var f = File.CreateText(repositoriesConf))
                    f.Flush();
            }
            else
            {
                using (var f = File.OpenText(repositoriesConf))
                {
                    while (!f.EndOfStream)
                    {
                        var line = f.ReadLine();
                        if (Repository.IsValidConfigString(line))
                            this.Repositories.AddRange(Repository.FromConfigString(line, this));
                    }
                }
            }

            // Operations collection
            this.Operations = new ObservableCollection<Operation>();
        }

        internal int GetOperationID()
        {
            int newOperationIndex = nextOperationIndexToAssign;
            nextOperationIndexToAssign++;
            return newOperationIndex;
        }

        // TODO: Repository caching? Complete? Not at all?

        // TODO: Implement FlagInstall - version flagging for "to install"
        // TODO: Implement FlagUninstall - version flagging for "to uninstall"

        // TODO: Implement GetOfflinePackageInfo - local package info fetching
        // TODO: Implement GetOnlinePackageInfo - remote package info fetching
        // TODO: Implement IsInstalled if needed - function to check if package is installed

        // TODO: Implement GetOnlineScript - remote package script fetching (private)
        // TODO: Implement CacheScript - remote to local package script caching (private)

    }
}
