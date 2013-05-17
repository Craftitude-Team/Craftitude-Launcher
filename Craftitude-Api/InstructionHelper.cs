#region Imports (5)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion Imports (5)

namespace Craftitude
{


    internal class InstructionHelper
    {
        internal DirectoryInfo _basePath;
        internal Client _client;

        internal InstructionHelper(Client mainClient)
        {
            this._client = mainClient;
            this._basePath = new DirectoryInfo(_client.BasePath);
        }

        public void DeleteDir(string targetdir)
        {
            _basePath.CreateSubdirectory(targetdir).Delete(true);
        }

        public void DeleteFile(string targetfile)
        {
            var f = new FileInfo(Path.Combine(_basePath.ToString(), targetfile));
            if (f.Exists)
                f.Delete();
        }

        public void Move(string source, string target)
        {
            var f = new FileInfo(Path.Combine(_basePath.ToString(), source));
            f.MoveTo(target);
        }
    }
}
