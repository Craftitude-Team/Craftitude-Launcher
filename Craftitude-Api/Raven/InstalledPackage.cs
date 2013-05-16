using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude.Raven
{
    class InstalledPackage : Package
    {
        public DateTime InstallDate { get; set; }
        public string[] WantedConditions { get; set; }
        public string InstalledVersionID { get; set; }
    }
}
