using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common
{
    public class Splitter<T> : INode
    {
        private Connection<T>? inputPort = null;
        private readonly List<Connection<T>> outputPorts = [];

        public INode SetPort(string portName, IConnection connection)
        {
            if (portName == "IN" && connection is Connection<T> con0) inputPort = con0;
            if (portName == "OUT" && connection is Connection<T> con1) outputPorts.Add(con1);
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
                foreach (var conn in outputPorts)
                    conn.Send(packet);
            }

            foreach (var conn in outputPorts)
                conn.Close();
        }
    }
}
