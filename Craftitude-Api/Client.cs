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

namespace Craftitude
{
    public class Client
    {
        public string BasePath { get; set; }
        public Cache Cache { get; set; }
        public List<Repository> Repositories { get; private set; }
        public ObservableCollection<Operation> Operations { get; set; } // TODO: Make operations list read-only to public

        private int nextOperationIndexToAssign = 0;

        /*

        public event EventHandler<OperationUpdateEventArgs> OperationUpdate;

        internal void OnOperationUpdate(OperationUpdateEventArgs e)
        {
            if (OperationUpdate != null)
                OperationUpdate.Invoke(this, e);
        }

        internal void OnOperationUpdate(Operation e)
        {
            OnOperationUpdate(new OperationUpdateEventArgs(e));
        }
        
         */

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
