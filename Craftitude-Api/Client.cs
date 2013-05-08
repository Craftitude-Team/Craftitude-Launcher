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

        public Client(string basePath)
        {
            if(!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            this.BasePath = Path.GetFullPath(basePath);
            this.Cache = new Cache(this, Path.Combine(this.BasePath, "craftitude.cache"));
        }

        public bool IsInstalled(string packageID)
        {
            return Cache.GetInstalledPackagesCache().GetPackage(packageID) != null;
        }

        // TODO: Implement client functions

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
