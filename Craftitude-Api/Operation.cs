#region Imports (4)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion Imports (4)

namespace Craftitude
{


    public class CollectiveOperation : Operation
    {
        public double Progress
        {
            get
            {
                return Suboperations.OfType<ProgressingOperation>().Average(p => p.Progress);
            }
        }

        public List<Operation> Suboperations { get; internal set; }

        public CollectiveOperation(Client client)
            : base(client)
        {
            this.Suboperations = new List<Operation>();
        }
    }

    public class Operation
    {
        public Client Client { get; private set; }

        public int ID { get; private set; }

        public string Text { get; internal set; }

        public Operation(Client client)
        {
            this.ID = client.GetOperationID();
            this.Client = client;
        }
    }

    public class ProgressingOperation : Operation
    {
        public double Progress { get; internal set; }

        public ProgressingOperation(Client client)
            : base(client)
        {
            Progress = 0;
        }
    }
}
