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
    }
}
