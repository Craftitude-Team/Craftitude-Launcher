using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    /// <summary>
    /// Represents the type of condition on how to handle the
    /// dependency itself.
    /// </summary>
    public enum DependencyType : byte
    {
        Requires = 0x00,
        Recommends = 0x01,
        Suggests = 0x02,

        NeedsForInstallation = 0x10 // Reserved for future usage
    }
}
