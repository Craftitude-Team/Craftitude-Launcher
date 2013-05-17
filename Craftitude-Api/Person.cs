using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    /// <summary>
    /// Represents a person.
    /// </summary>
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
        public Uri Url { get; protected set; }
    }
}
