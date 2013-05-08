using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Craftitude
{
    abstract class Metadata
    {
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
        public Dictionary<string, VersionMetadata> Versions
        {
            get;
            protected set;
        }
    }
}
