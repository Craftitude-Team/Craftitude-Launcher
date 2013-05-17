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
    /// <summary>
    /// Represents package information.
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// Metadata for this package.
        /// </summary>
        public PackageMetadata Metadata { get; private set; }
    }

    /// <summary>
    /// Represents a version of an online package.
    /// </summary>
    public class PackageVersion
    {
        /// <summary>
        /// The ID of this version. Usually the version number.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The date on which this version has been published.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The dependencies of this version.
        /// </summary>
        public List<Dependency> Dependencies { get; set; }

        /// <summary>
        /// The installation script, an actual Lua script.
        /// This script will be executed when the package is going to be installed.
        /// </summary>
        public string InstallScript { get; set; }

        /// <summary>
        /// The uninstallation script, an actual Lua script.
        /// This script will be executed when the package is going to be uninstalled.
        /// </summary>
        public string UninstallScript { get; set; }

        /// <summary>
        /// The startup script, an actual Lua script.
        /// This script will be executed to append required files to the startup command-line of
        /// java.
        /// </summary>
        public string StartupScript { get; set; }
    }

    /// <summary>
    /// Represents online package metadata.
    /// </summary>
    public class PackageMetadata
    {
        /// <summary>
        /// The package's description. May be null if not given.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The list of people which develop the package's contents.
        /// </summary>
        public List<Person> Developers { get; protected set; }

        /// <summary>
        /// The homepage of that package. May be null if not given.
        /// </summary>
        public string Homepage { get; protected set; }

        /// <summary>
        /// The people which are maintaining the package on the repository.
        /// </summary>
        public List<Person> Maintainers { get; protected set; }

        /// <summary>
        /// The package's name.
        /// </summary>
        public string Name { get; protected set; }

    }
}
