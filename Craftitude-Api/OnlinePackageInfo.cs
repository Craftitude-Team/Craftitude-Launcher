using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Json;
using Raven.Json.Linq;
using Raven.Client.Document;
using Raven.Storage;
using Raven.Imports.Newtonsoft.Json;

namespace Craftitude
{
    public class OnlinePackageInfo : RavenJObject
    {
        public OnlinePackageInfo()
            : base()
        {
            Metadata = new OnlinePackageMetadata(this);
        }

        internal DocumentSession _activeSession { get; set; }

        public OnlinePackageMetadata Metadata { get; private set; }
    }

    public class OnlinePackageMetadata
    {
        OnlinePackageInfo _obj;

        public OnlinePackageMetadata(OnlinePackageInfo obj)
        {
            this._obj = obj;
        }

        private RavenJObject _metadata { get { return _obj._activeSession.Advanced.GetMetadataFor(_obj); } }

        public string Name { get { return _metadata["Name"].Value<string>(); } }
        public string Description { get { return _metadata["Description"].Value<string>(); } }
        public string Homepage { get { return _metadata["Homepage"].Value<string>(); } }
        public List<Person> Maintainers { get { return _metadata["Maintainers"].Value<List<Person>>(); } }
        public List<Person> Developers { get { return _metadata["Developers"].Value<List<Person>>(); } }
    }

    public class Person
    {
        public string Name { get; protected set; }
        public string RealName { get; protected set; }
        public string Url { get; protected set; }
        public string EMail { get; protected set; }
    }
}
