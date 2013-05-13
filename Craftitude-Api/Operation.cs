using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    public class Operation
    {
        public int ID { get; private set; }
        public Client Client { get; private set; }
        public string Text { get; internal set; }

        public Operation(Client client)
        {
            this.ID = client.GetOperationID();
            this.Client = client;
        }
    }

    public class CollectiveOperation : Operation
    {
        public List<Operation> Suboperations { get; internal set; }

        public double Progress
        {
            get
            {
                return Suboperations.OfType<ProgressingOperation>().Average(p => p.Progress);
            }
        }

        public CollectiveOperation(Client client)
            : base(client)
        {
            this.Suboperations = new List<Operation>();
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

    /*
    public class OperationUpdateEventArgs
    {
        public Operation Operation { get; private set; }

        public OperationUpdateEventArgs(Operation operation)
        {
            this.Operation = operation;
        }
    }
     */

}
