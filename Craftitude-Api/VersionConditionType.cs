using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    /// <summary>
    /// Represents the type of condition on how to handle the
    /// given version of a dependency.
    /// </summary>
    public enum VersionConditionType : byte
    {
        EqualTo = 0,
        NewerThan = 1,
        OlderThan = 2,
        NewerThanOrEqualTo = 3,
        OlderThanOrEqualTo = 4,
        NotEqualTo = 5
    }
}
