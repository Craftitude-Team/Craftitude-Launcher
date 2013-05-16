using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude.Raven
{
    public class Package
    {
        // Metadata
        public string Name
        {
            get;
            protected set;
        }
        public string Description
        {
            get;
            protected set;
        }
        public string[] Maintainers
        {
            get;
            protected set;
        }
        public string[] Developers
        {
            get;
            protected set;
        }
        public License License
        {
            get;
            protected set;
        }
        public string[] Platforms
        {
            get;
            protected set;
        }
        public string[] Dependencies
        {
            get;
            protected set;
        }

        // Version tree
        public PackageVersion[] Versions
        {
            get;
            protected set;
        }

        // Scripts
        public string InstallScript { get; set; }
        public string UninstallScript { get; set; }
        public string StartupScript { get; set; }
    }
}
