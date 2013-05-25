using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Raven;
using Raven.Abstractions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database;

namespace Craftitude
{
    public class CraftitudeClient
    {
        public CraftitudeClient(string basePath)
        {
            this.Cache = new CacheDatabase(basePath);
        }

        public CacheDatabase Cache { get; internal set; }
        public class CacheDatabase
        {
            internal EmbeddableDocumentStore _store;

            public CacheDatabase(string path)
            {
                _store = new EmbeddableDocumentStore()
                {
                    DataDirectory = path,
                    DefaultDatabase = "installation"
                };
                _store.Initialize();
                var existantDatabases = _store.DatabaseCommands.GetDatabaseNames(8);

                if (!existantDatabases.Contains("installation"))
                    _store.DatabaseCommands.CreateDatabase(new DatabaseDocument()
                    {
                        Id = "installation",
                        Settings = {
                            { "Raven/ActiveBundles", "Compression" }
                        }
                    });
            }
        }

        public List<Repository> Repositories = new List<Repository>();
        public class Repository
        {
            internal EmbeddableDocumentStore _cacheStore;
            internal DocumentStore _store;

            public Repository(Uri uri, string cachePath)
            {
                if (!uri.Scheme.Equals("crep", StringComparison.OrdinalIgnoreCase))
                    throw new UriFormatException("This URI has an invalid scheme: \"" + uri.ToString() + "\". Scheme must be \"crep\".");

                var split = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (!split.Any())
                    throw new UriFormatException("This URI doesn't contain a repository name: \"" + uri.ToString() + "\". It must at least contain the repository's name.");

                // Get database name and remove it from path
                var splitStack = new Stack<string>(split.Reverse());
                var dbName = splitStack.Pop();

                // Rebuild to RavenDB http url
                UriBuilder ub = new UriBuilder(uri);
                ub.Path = "/" + string.Join("/", splitStack.Reverse());
                ub.Scheme = "http";

                // Load remote RavenDB
                _store = new DocumentStore()
                {
                    Url = ub.Uri.ToString()
                };
                _store.Initialize();

                // Load local RavenDB (cache)
                _cacheStore = new EmbeddableDocumentStore()
                {
                    DataDirectory = cachePath,
                    DefaultDatabase = GetGuidForUri(uri).ToString()
                };
                _cacheStore.Initialize();

                // Check if database for repository already exists
                var existantDatabases = new string[32];
                int position = 0;

                while (existantDatabases.Count() == 32)
                {
                    existantDatabases = _cacheStore.DatabaseCommands.GetDatabaseNames(32, position);

                    if (!existantDatabases.Contains(_cacheStore.DefaultDatabase))
                        position += existantDatabases.Count();
                }

                // Create database for this repository if not existing
                if (!existantDatabases.Contains(_cacheStore.DefaultDatabase))
                {
                    _cacheStore.DatabaseCommands.CreateDatabase(new DatabaseDocument()
                    {
                        Id = _cacheStore.DefaultDatabase,
                        Settings = {
                            { "Raven/ActiveBundles", "Compression" }
                        }
                    });
                }
            }

            protected Guid GetGuidForUri(Uri uri)
            {
                return GetGuidForUri(uri.ToString());
            }

            protected Guid GetGuidForUri(string uri)
            {
                // A usable index name for RavenDB
                var uriindex = BitConverter.ToString(Encoding.UTF8.GetBytes(uri)).Replace("-", "");

                // Look if guid is already available for this uri
                using (var session = _cacheStore.OpenSession("guidindex"))
                {
                    var q = session.Query<Guid>(uriindex);
                    if (q.Any())
                        return q.Single();
                    else
                    {
                        var guid = new Guid();
                        session.Store(guid, uriindex);
                        session.SaveChanges();
                        return guid;
                    }
                }
            }
        }

        public class Package
        {
            public string Name { get; internal set; }

            public string Description { get; internal set; }

            public string Homepage { get; internal set; }

            public string Version { get; internal set; }

            public List<Person> Developers { get; internal set; }

            public List<Person> Maintainers { get; internal set; }

            public List<Version> Versions { get; internal set; }
            public class Version
            {
                public string ID { get; internal set; }

                public License License { get; internal set; }
                public class License
                {
                    public string Name { get; set; }
                    public string Url { get; set; }
                    public string Text { get; set; }
                }

                public List<Condition> Conditions { get; internal set; }
                public class Condition
                {
                    public ConditionType Type { get; internal set; }
                    public enum ConditionType
                    {
                        Requires = 1,
                        Recommends = 2,
                        Suggests = 3,
                        NotCompatibleWith = 4,
                        //RequiresForInstallation = 5,
                    }

                    public string PackageName { get; internal set; }

                    public List<VersionCondition> PackageVersions { get; internal set; }
                    public class VersionCondition
                    {
                        public VersionConditionType ConditionType { get; internal set; }
                        public enum VersionConditionType
                        {
                            Equal,
                            Newer,
                            NewerOrEqual,
                            Older,
                            OlderOrEqual,
                            Not,
                            Regex
                        }

                        public string Version { get; internal set; }
                    }
                }

                public List<string> Platforms { get; internal set; }

                public List<string> Distributions { get; internal set; }

                public string InstallScript { get; internal set; }

                public string UninstallScript { get; internal set; }

                public string StartupScript { get; internal set; }
            }
        }

        public class InstalledPackage : LocalPackage
        {
            public bool ManuallyInstalled { get; internal set; }

            public string InstalledVersion { get; internal set; }
        }

        public class SearchResult
        {
            public CraftitudeClient Client { get; internal set; }

            public Repository Repository { get; internal set; }

            public Package OriginalPackage { get; internal set; }

            public IEnumerable<Package.Version> FoundVersions { get; internal set; }
        }

        public class Person
        {
            public string RealName { get; internal set; }

            public string Nickname { get; internal set; }

            public string EMailAddress { get; internal set; }

            public List<string> Urls { get; internal set; }
        }

        public IEnumerable<InstalledPackage> GetInstalledPackages()
        {
            using (var session = Cache._store.OpenSession())
            {
                return session.Query<InstalledPackage>();
            }
        }

        public IEnumerable<Package.Version> SearchPackageVersions(string name, params Package.Version.Condition.VersionCondition[] versionConditions)
        {
            return this.SearchPackageVersions(name, versionConditions.AsEnumerable());
        }

        public IEnumerable<Package.Version> SearchPackageVersions(string name, IEnumerable<Package.Version.Condition.VersionCondition> versionConditions)
        {
            return this.SearchPackageVersions(SearchPackagesByName(name).Single(), versionConditions);
        }

        public IEnumerable<Package.Version> SearchPackageVersions(Package package, params Package.Version.Condition.VersionCondition[] versionConditions)
        {
            return this.SearchPackageVersions(package, versionConditions.AsEnumerable());
        }

        public IEnumerable<Package.Version> SearchPackageVersions(Package package, IEnumerable<Package.Version.Condition.VersionCondition> versionConditions)
        {
            var versions = package.Versions;
            foreach (var condition in versionConditions)
            {
                versions.RemoveAll(v =>
                {
                    bool r;
                    switch (condition.ConditionType)
                    {
                        case Package.Version.Condition.VersionCondition.VersionConditionType.Equal:
                            r = v.ID == condition.Version;
                            break;
                        case Package.Version.Condition.VersionCondition.VersionConditionType.Newer:
                            r = versions.IndexOf(v) > versions.IndexOf(versions.Single(sv => sv.ID.Equals(condition.Version)));
                            break;
                        case Package.Version.Condition.VersionCondition.VersionConditionType.NewerOrEqual:
                            r = versions.IndexOf(v) >= versions.IndexOf(versions.Single(sv => sv.ID.Equals(condition.Version)));
                            break;
                        case Package.Version.Condition.VersionCondition.VersionConditionType.Not:
                            r = versions.IndexOf(v) != versions.IndexOf(versions.Single(sv => sv.ID.Equals(condition.Version)));
                            break;
                        case Package.Version.Condition.VersionCondition.VersionConditionType.Older:
                            r = versions.IndexOf(v) < versions.IndexOf(versions.Single(sv => sv.ID.Equals(condition.Version)));
                            break;
                        case Package.Version.Condition.VersionCondition.VersionConditionType.OlderOrEqual:
                            r = versions.IndexOf(v) <= versions.IndexOf(versions.Single(sv => sv.ID.Equals(condition.Version)));
                            break;
                        default: // regex
                            r = Regex.IsMatch(
                                v.ID,
                                Regex.Escape(condition.Version).Replace(@"\*", ".*").Replace(@"\?", ".") // 1.* => 1\..*
                            );
                            break;
                    }
                    return !r;
                });
            }
            return versions;
        }

        public IEnumerable<Package> SearchPackagesByName(string name)
        {
            foreach (var repo in Repositories)
            {
                using (var session = repo._store.OpenSession())
                {
                    var packages = session.Query<Package>()
                        .Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    foreach (var package in packages)
                        yield return package;
                }
            }
        }

        public Dictionary<Package.Version.Condition.ConditionType, List<SearchResult>> SearchPackagesByConditions(params Package.Version.Condition[] conditions)
        {
            return SearchPackagesByConditions(conditions.AsEnumerable());
        }

        public Dictionary<Package.Version.Condition.ConditionType, List<SearchResult>> SearchPackagesByConditions(IEnumerable<Package.Version.Condition> conditions)
        {
            var ret = new Dictionary<Package.Version.Condition.ConditionType, List<SearchResult>>();

            foreach (var repo in Repositories)
            {
                using (var session = repo._store.OpenSession())
                {
                    // Query all packages in this repo. Hopefully RavenDB caches
                    // this properly. I think so.
                    var packagesQueryAsync = session.Query<Package>().ToList();
                    foreach (var condition in conditions)
                    {
                        // Create empty SearchResult list for yet non-existant condition output
                        if (!ret.ContainsKey(condition.Type))
                        {
                            ret.Add(condition.Type, new List<SearchResult>());
                        }

                        // Get all versions by using whole package information
                        var packagesForName = this.SearchPackagesByName(condition.PackageName);

                        foreach (var package in packagesForName)
                        {
                            SearchResult res = new SearchResult();
                            res.Client = this;
                            res.Repository = repo;
                            res.OriginalPackage = package;
                            res.FoundVersions = new List<Package.Version>();

                            // Go through all matching versions
                            foreach (Package.Version version in SearchPackageVersions(package, condition.PackageVersions))
                            {
                                // Add this version to current condition
                                ((List<Package.Version>)res.FoundVersions).Add(version);

                                // Remove exactly this version in other conditions
                                foreach (var c in ret.Keys.Where(k => !k.Equals(condition.Type)))
                                    foreach (var r in ret[c])
                                        ((List<Package.Version>)r.FoundVersions).RemoveAll(m => m == version);
                            }
                        }
                    }
                }
            }

            return ret;
        }
    }
}
