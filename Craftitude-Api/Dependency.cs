using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    /// <summary>
    /// Represents a package version's dependency.
    /// </summary>
    public class Dependency
    {
        public string PackageID { get; set; }
        public string Version { get; set; }
        public DependencyType DependencyType { get; set; }
        public VersionConditionType VersionConditionType { get; set; }
    }
}
