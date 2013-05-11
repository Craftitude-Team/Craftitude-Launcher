using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    public class Repository
    {
        public string Url { get; set; }
        private string HttpUrl { get { return new UriBuilder(Url) { Scheme = "http" }.ToString(); } }
        public string Distribution { get; set; }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Url, Distribution);
        }

        internal string ToConfigString()
        {
            return string.Format("repository {0} {1}", Url, Distribution);
        }

        internal static bool IsValidConfigString(string configString)
        {
            var s = configString.Split(' ');
            if (s.Count() < 3 || s[0] != "repository" || !Uri.IsWellFormedUriString(s[1], UriKind.Absolute) || !s[1].StartsWith("crep://", StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }

        internal static IEnumerable<Repository> FromConfigString(string configString, Client client)
        {
            if (!IsValidConfigString(configString))
                throw new InvalidOperationException("Not a valid repository configuration line: " + configString);

            var s = configString.Split(' ');

            foreach (string dist in s.Skip(2))
                yield return new Repository() {
                    Url = s[1],
                    Distribution = s[2],
                    Cache = client.Cache.GetRepositoryCache(new Uri(s[1]))
                };
        }

        internal Repository() { }

        internal string GetPackagesYmlUrl()
        {
            return string.Format("{0}/{1}", this.ToString(), "packages.yml");
        }

        internal string GetPatchUrl(ulong fromVersion, ulong toVersion)
        {
            return string.Format("{0}/packages_{0}_to_{1}.patch", this.ToString(), fromVersion, toVersion);
        }

        internal string GetPackageUrl(string package)
        {
            return string.Format("{0}/packages/{1}", this.ToString(), package);
        }

        internal string GetPackageVersionUrl(string package, string version)
        {
            return string.Format("{0}/{1}", GetPackageUrl(package), version);
        }

        internal string GetPackageLuaUrl(string package, string version, string luaName)
        {
            return string.Format("{0}/{1}", GetPackageVersionUrl(package, version), luaName);
        }

        internal RepositoryCache Cache { get; set; }

    }
}
