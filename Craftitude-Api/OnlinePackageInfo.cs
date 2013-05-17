#region Imports (9)

using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json;
using Raven.Json.Linq;
using Raven.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion Imports (9)

namespace Craftitude
{


    public class OnlinePackageInfo : RavenJObject
    {
        internal DocumentSession _activeSession { get; set; }

        public OnlinePackageMetadata Metadata { get; private set; }

        public OnlinePackageInfo()
            : base()
        {
            Metadata = new OnlinePackageMetadata(this);
        }
    }

    public class OnlinePackageMetadata
    {
        OnlinePackageInfo _obj;

        private RavenJObject _metadata { get { return _obj._activeSession.Advanced.GetMetadataFor(_obj); } }

        public string Description { get { return _metadata["Description"].Value<string>(); } }

        public List<Person> Developers { get { return _metadata["Developers"].Value<List<Person>>(); } }

        public string Homepage { get { return _metadata["Homepage"].Value<string>(); } }

        public List<Person> Maintainers { get { return _metadata["Maintainers"].Value<List<Person>>(); } }

        public string Name { get { return _metadata["Name"].Value<string>(); } }

        public OnlinePackageMetadata(OnlinePackageInfo obj)
        {
            this._obj = obj;
        }
    }

    public class Person
    {
        public string EMail { get; protected set; }

        public string Name { get; protected set; }

        public string RealName { get; protected set; }

        public string Url { get; protected set; }
    }
}
