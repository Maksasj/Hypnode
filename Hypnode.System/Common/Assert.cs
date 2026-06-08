using Hypnode.Core;

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

        public async Task ExecuteAsync()
        {
            if (inputPort is null)
                throw new InvalidOperationException("Input port is not set");

            var packet = inputPort.Receive();

            if (!packet)
                throw new InvalidOperationException("Assertion failed");
        }
    }
}
