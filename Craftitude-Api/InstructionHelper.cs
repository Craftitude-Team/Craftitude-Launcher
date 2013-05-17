using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Craftitude
{
    internal class InstructionHelper
    {
        internal InstructionHelper(Client mainClient)
        {
            this._client = mainClient;
            this._basePath = new DirectoryInfo(_client.BasePath);
        }

        internal Client _client;
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
            var f = new FileInfo(Path.Combine(_basePath.ToString(), source));
            f.MoveTo(target);
        }

        // TODO: Complete instruction set
    }
}
