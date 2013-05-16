using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude.Raven
{
    public class PackageVersion
    {
        public string[] Distributions
        {
            get;
            protected set;
        }

        public string InstallScript { get; set; }
        public string UninstallScript { get; set; }
        public string StartupScript { get; set; }
    }
}
