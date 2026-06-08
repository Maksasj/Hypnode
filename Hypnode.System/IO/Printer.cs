using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.IO
{
    public class Printer<T> : INode
    {
        private Connection<T>? inputPort = null;

        public INode SetPort(string portName, IConnection connection)
        {
            if (portName == "IN" && connection is Connection<T> conn) inputPort = conn;
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

                Console.WriteLine($"{inputPort.Receive()}");
            }
        }
    }
}
