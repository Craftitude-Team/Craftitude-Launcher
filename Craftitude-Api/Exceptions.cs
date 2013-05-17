using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    /*
    [Serializable]
    public class PackageDuplicatesAvailableException : Exception
    {
        public IEnumerable<PackageInfo> ConflictingPackages { get; set; }

        private PackageDuplicatesAvailableException()
        { }

        public PackageDuplicatesAvailableException(PackageInfo[] conflictingPackages)
            : this(conflictingPackages.AsEnumerable())
        { }

        public PackageDuplicatesAvailableException(IEnumerable<PackageInfo> conflictingPackages)
        {
            this.ConflictingPackages = conflictingPackages;
        }

        public PackageDuplicatesAvailableException(string message, PackageInfo[] conflictingPackages)
            : base(message)
        {
            this.ConflictingPackages = conflictingPackages;
        }

        public PackageDuplicatesAvailableException(string message, PackageInfo[] conflictingPackages, Exception inner)
            : base(message, inner)
        {
            this.ConflictingPackages = conflictingPackages;
        }

        protected PackageDuplicatesAvailableException(
          PackageInfo[] conflictingPackages,
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.ConflictingPackages = conflictingPackages;
        }
    }
    */

    [Serializable]
    public class LocalCacheCorruptException : Exception
    {
        public LocalCacheCorruptException() { }
        public LocalCacheCorruptException(string message) : base(message) { }
        public LocalCacheCorruptException(string message, Exception inner) : base(message, inner) { }
        protected LocalCacheCorruptException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}