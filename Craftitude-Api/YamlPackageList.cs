using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YaTools.Yaml;
using YaTools.Yaml.AbstractContracts;

namespace Craftitude
{
    public class YamlPackageList
    {
        public ulong Version { get; set; }
        public IEnumerable<YamlPackage> Packages { get; set; }
    }

    public class YamlPackage : Package
    {
    }
}