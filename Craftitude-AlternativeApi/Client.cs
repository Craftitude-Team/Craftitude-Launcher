using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database;

namespace Craftitude
{
    public class CraftitudeClient
    {
        public class Cache
        {
            EmbeddableDocumentStore _store;

            public Cache(string path)
            {
                _store = new EmbeddableDocumentStore()
                {
                    DataDirectory = path,
                    DefaultDatabase = "installation"
                };
                _store.Initialize();
                var existantDatabases = _store.DatabaseCommands.GetDatabaseNames(8);

                if (!existantDatabases.Contains("installation"))
                    _store.DatabaseCommands.CreateDatabase(new Raven.Abstractions.Data.DatabaseDocument()
                    {
                        Id = "installation",
                        Settings = {
                            { "Raven/ActiveBundles", "Compression" }
                        }
                    });
            }
        }

        public class Package
        {
            public string Name { get; internal set; }
            public string Description { get; internal set; }
            public string Homepage { get; internal set; }
            public List<Person> Developers { get; internal set; }
            public List<Person> Maintainers { get; internal set; }

            public List<Version> Versions { get; internal set; }

            public class Version
            {
                public License License { get; internal set; }
                public List<string> Platforms { get; internal set; }
                public List<string> Distributions { get; internal set; }

                public string InstallScriptId { get; internal set; }
                public string UninstallScriptId { get; internal set; }
                public string StartupScriptId { get; internal set; }

                public class License
                {
                    public string Name { get; set; }
                    public string Url { get; set; }
                    public string Text { get; set; }
                }
            }
        }

        public class Person
        {
            public string RealName { get; internal set; }
            public string Nickname { get; internal set; }
            public string EMailAddress { get; internal set; }
            public List<string> Urls { get; internal set; }
        }
    }
}
