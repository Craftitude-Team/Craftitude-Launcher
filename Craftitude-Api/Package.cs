using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using YaTools.Yaml;
using AluminumLua;
using AluminumLua.Executors;
using AluminumLua.Executors.ExpressionTrees;
using Raven.Imports.Newtonsoft.Json;

namespace Craftitude
{
    public class Package : Metadata
    {
        [JsonIgnore]
        internal RepositoryCache _repositoryCache { get; set; }
    }

    public class RepositoryPackage : Package
    {

    }

    public class InstalledPackage : Package
    {
        public string InstalledVersionID { get; set; }
        public string InstallScript { get; set; }
        public string UninstallScript { get; set; }
        public string StartupScript { get; set; }

        private void LuaScript(TextReader script, Dictionary<string, object> additionalVariables = null)
        {
            var instructionHelper = new InstructionHelper(this);
            var context = new LuaContext();
            context.AddBasicLibrary();

            // Import methods from InstructionHelper
            foreach(var method in
                from m in instructionHelper.GetType().GetMethods()
                where
                    m.IsPublic
                    && m.ReturnParameter.ParameterType == typeof(LuaObject)
                    && m.GetParameters().Count() == 1
                    && m.GetParameters().First().ParameterType == typeof(LuaObject)
                select m)
            {
                var function = (LuaFunction)Delegate.CreateDelegate(typeof(LuaFunction), method);
                context.SetGlobal(method.Name, function);
            }

            // Import variables
            context.SetGlobal("package", LuaObject.FromObject(this));
            if (additionalVariables != null)
                foreach (string k in additionalVariables.Keys)
                    context.SetGlobal(k, LuaObject.FromObject(additionalVariables[k]));

            var compiler = new DefaultExecutor(context);
            
            var parser = new LuaParser(compiler, script);
            parser.Parse(false);
        }

        internal void RunInstall()
        {
            LuaScript(new StringReader(InstallScript));
        }

        internal void RunUninstall()
        {
            LuaScript(new StringReader(UninstallScript));
        }

        internal void RunStartup(PrioritizedList<int, string> libraryPaths, PrioritizedList<int, string> classPaths)
        {
            LuaScript(new StringReader(UninstallScript), new Dictionary<string, object>()
            {
                { "librarypaths", libraryPaths },
                { "classpaths", classPaths }
            });
        }
    }

    internal class InstructionHelper
    {
        internal InstructionHelper(Package package)
        {
            this._package = package;
            this._basePath = new DirectoryInfo(_package._repositoryCache._cache._client.BasePath);
        }

        internal Package _package;
        internal DirectoryInfo _basePath;

        public void DeleteFile(string targetfile)
        {
            var f = new FileInfo(Path.Combine(_basePath.ToString(), targetfile));
            if (f.Exists)
                f.Delete();
        }

        public void DeleteDir(string targetdir)
        {
            _basePath.CreateSubdirectory(targetdir).Delete(true);
        }

        public void Move(string source, string target)
        {
        }
    }
}
