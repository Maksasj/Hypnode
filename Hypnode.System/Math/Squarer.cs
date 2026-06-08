using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Math
{
    public class Squarer : INode
    {
        private Connection<int>? inputPort = null;
        private Connection<int>? outputPort = null;

        public INode SetPort(string portName, IConnection connection)
        {
            if (portName == "IN" && connection is Connection<int> con0) inputPort = con0;
            if (portName == "OUT" && connection is Connection<int> con1) outputPort = con1;
            return this;
        }

        public IEnumerator Execute()
        {
            if (inputPort is null)
                throw new InvalidOperationException("Input port is not set");

            while (true)
            {
                if (inputPort.IsClosed && !inputPort.HasData)
                    break;

                if (!inputPort.HasData)
                {
                    yield return null;
                    continue;
                }

                var packet = inputPort.Receive();
                outputPort?.Send(packet * packet);
            }

            outputPort?.Close();
        }
    }
}
