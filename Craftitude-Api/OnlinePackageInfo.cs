﻿#region Imports (9)

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
        public PackageMetadata Metadata { get; internal set; }

        /// <summary>
        /// Available package versions.
        /// </summary>
        public Dictionary<string, PackageVersion> Versions { get; internal set; }

        /// <summary>
        /// Returns versions matching specific conditions.
        /// </summary>
        /// <param name="conditions">The conditions to match the available versions against.</param>
        /// <returns></returns>
        internal IEnumerable<PackageVersion> FilterVersions(params string[] conditions)
        {
            // All versions.
            var versions = this.Versions.ToList();

            // Apply each conditions one by one
            // TODO: Allow wildcards?
            foreach (var condition in conditions)
            {
                string target = condition;

                bool negative = false;
                if(target.First() == '!')
                {
                    negative = true;
                    target = target.Substring(1);
                }

                List<char> operators = new List<char>();
                do
                {
                    switch (condition.First())
                    {
                        case '>':
                        case '=':
                        case '<':
                            operators.Add(condition.First());
                            target = target.Substring(1);
                            continue;
                    }
                } while (false);

                string @operator = new string(operators.ToArray());

                switch (comparisonOperator)
                {
                    case '!': // not equal
                        {
                            versions = versions.Where(i => !i.Key.Equals(comparisonTarget)) as Dictionary<string, PackageVersion>;
                        } break;
                    case '=': // equal
                        {
                            versions = versions.Where(i => i.Key.Equals(comparisonTarget)) as Dictionary<string, PackageVersion>;
                        } break;
                    case '>': // newer than
                        {
                            var comparisonTargetIndex = GetVersionIndex(comparisonTarget);
                            
                            versions = versions.Where(i => GetVersionIndex(i.Key) > comparisonTargetIndex) as Dictionary<string, PackageVersion>;
                        } break;
                    case '<': // older than
                        {
                            var comparisonTargetIndex = GetVersionIndex(comparisonTarget);
                            versions = versions.Where(i => GetVersionIndex(i.Key) > comparisonTargetIndex) as Dictionary<string, PackageVersion>;
                        } break;
                }
            }
        }

        /// <summary>
        /// Gets the index of a specific version ID.
        /// </summary>
        /// <param name="versionID">The specific version ID</param>
        /// <returns></returns>
        internal int GetVersionIndex(string versionID)
        {
            return Versions.Keys.ToList().IndexOf(versionID);
        }
    }

    /// <summary>
    /// Represents locally cached package information.
    /// </summary>
    public class LocalPackageInfo : PackageInfo
    {
        /// <summary>
        /// Gets when this package has been installed.
        /// </summary>
        public DateTime InstallationTime { get; internal set; }

        /// <summary>
        /// Gets under which conditions this package has been installed.
        /// </summary>
        public string WantedConditions { get; internal set; }

        /// <summary>
        /// Gets which version of this package is currently installed.
        /// </summary>
        public string InstalledVersionID { get; internal set; }
    }

    /// <summary>
    /// Represents a version of an online package.
    /// </summary>
    public class PackageVersion
    {
        /// <summary>
        /// The ID of this version. Usually the version number.
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// The date on which this version has been published.
        /// </summary>
        public DateTime Date { get; internal set; }

        /// <summary>
        /// The dependencies of this version.
        /// </summary>
        public List<Dependency> Dependencies { get; internal set; }

        /// <summary>
        /// The installation script, an actual Lua script.
        /// This script will be executed when the package is going to be installed.
        /// </summary>
        public string InstallScript { get; internal set; }

        /// <summary>
        /// The uninstallation script, an actual Lua script.
        /// This script will be executed when the package is going to be uninstalled.
        /// </summary>
        public string UninstallScript { get; internal set; }

        /// <summary>
        /// The startup script, an actual Lua script.
        /// This script will be executed to append required files to the startup command-line of
        /// java.
        /// </summary>
        public string StartupScript { get; internal set; }
    }

    /// <summary>
    /// Represents online package metadata.
    /// </summary>
    public class PackageMetadata
    {
        /// <summary>
        /// The package's description. May be null if not given.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The list of people which develop the package's contents.
        /// </summary>
        public List<Person> Developers { get; internal set; }

        /// <summary>
        /// The homepage of that package. May be null if not given.
        /// </summary>
        public string Homepage { get; internal set; }

        /// <summary>
        /// The people which are maintaining the package on the repository.
        /// </summary>
        public List<Person> Maintainers { get; internal set; }

        /// <summary>
        /// The package's name.
        /// </summary>
        public string Name { get; internal set; }

    }
}
