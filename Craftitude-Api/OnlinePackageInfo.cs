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
        /// <summary>
        /// The session via which the package's information is fetched.
        /// </summary>
        internal DocumentSession _activeSession { get; set; }

        /// <summary>
        /// Metadata for this package.
        /// </summary>
        public OnlinePackageMetadata Metadata { get; private set; }

        internal OnlinePackageInfo()
            : base()
        {
            Metadata = new OnlinePackageMetadata(this);
        }
    }

    public class OnlinePackageMetadata
    {
        OnlinePackageInfo _obj;

        private RavenJObject _metadata { get { return _obj._activeSession.Advanced.GetMetadataFor(_obj); } }

        /// <summary>
        /// The package's description. May be null if not given.
        /// </summary>
        public string Description { get { return _metadata["Description"].Value<string>(); } }

        /// <summary>
        /// The list of people which develop the package's contents.
        /// </summary>
        public List<Person> Developers { get { return _metadata["Developers"].Value<List<Person>>(); } }

        /// <summary>
        /// The homepage of that package. May be null if not given.
        /// </summary>
        public string Homepage { get { return _metadata["Homepage"].Value<string>(); } }

        /// <summary>
        /// The people which are maintaining the package on the repository.
        /// </summary>
        public List<Person> Maintainers { get { return _metadata["Maintainers"].Value<List<Person>>(); } }

        /// <summary>
        /// The package's name.
        /// </summary>
        public string Name { get { return _metadata["Name"].Value<string>(); } }

        internal OnlinePackageMetadata(OnlinePackageInfo obj)
        {
            this._obj = obj;
        }
    }

    public class Person
    {
        /// <summary>
        /// The e-mail address of that person. May be null if not given.
        /// </summary>
        public string EMail { get; protected set; }

        /// <summary>
        /// The nickname or alias of that person.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The real name of that person. May be null if not given.
        /// </summary>
        public string RealName { get; protected set; }

        /// <summary>
        /// The url referring to a profile of that person. May be null if not given.
        /// </summary>
        public string Url { get; protected set; }
    }
}
