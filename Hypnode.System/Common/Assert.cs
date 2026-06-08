using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.IO
{
    public class Assert : INode
    {
        private Connection<bool>? inputPort = null;

        public INode SetPort(string portName, IConnection connection)
        {
            if (portName == "IN" && connection is Connection<bool> con) inputPort = con;
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

                if (!inputPort.Receive())
                    throw new InvalidOperationException("Assertion failed");
            }
        }
    }
}
