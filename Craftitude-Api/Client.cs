using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Craftitude
{
    public class Client
    {
        public string BasePath { get; set; }
        public Cache Cache { get; set; }
        public List<Repository> Repositories { get; set; }

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
            string repositoriesConf = Path.Combine(craftitudeBasePath, "repositories.conf");
            if (!File.Exists(repositoriesConf))
            {
                using (var f = File.CreateText(repositoriesConf))
                {
                    f.WriteLine("# You need to put all Craftitude repositories you want to use here.");
                    f.WriteLine("# All repositories which are put here should be added in this form:");
                    f.WriteLine("#");
                    f.WriteLine("# repository crep://repository.serv.er/path/to/repository distribution another-optional-distribution");
                    f.WriteLine("#");
                    f.WriteLine("# Example:");
                    f.WriteLine("#");
                    f.WriteLine("# repository crep://craftitude.customserver.com/modifications client-1.5.1 server-1.5.1");
                    f.WriteLine("#");
                    f.WriteLine("# ...would be the same as...");
                    f.WriteLine("#");
                    f.WriteLine("# repository crep://craftitude.customserver.com/modifications client-1.5.1");
                    f.WriteLine("# repository crep://craftitude.customserver.com/modifications server-1.5.1");
                    f.WriteLine("#");
                    f.Flush();
                }
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
        }

        public void UpdateCache()
        {
            foreach (var repo in Repositories)
            {
                
            }
        }

        public bool IsInstalled(string packageID)
        {
            return Cache.GetInstalledPackagesCache().GetPackage(packageID) != null;
        }

        public void Install(string packageID)
        {
        }

        public void Uninstall(string packageID)
        {
        }

        public void Update(string packageID)
        {
        }

        public RepositoryPackage GetOnlinePackageInfo(string packageID)
        {

        }

        public InstalledPackage GetOfflinePackageInfo(string packageID)
        {
        }

        public IEnumerable<InstalledPackage> GetInstalledPackages()
        {
            
        }
    }
}
