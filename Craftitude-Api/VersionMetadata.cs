using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Craftitude
{
    abstract class VersionMetadata : Metadata
    {
        public string Date
        {
            get;
            set;
        }

        public DateTime ConvertedDate
        {
            get
            {
                return DateTime.ParseExact(
                    Date,
                    new string[] { "s", "u" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
            }
        }
    }
}
