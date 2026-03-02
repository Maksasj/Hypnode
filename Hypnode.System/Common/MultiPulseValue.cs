using Hypnode.Core;

namespace Hypnode.System.Common
{
    public class MultiPulseValue<T> : INode
    {
        private IEnumerable<T> Value { get; set; }

        private Connection<T>? outputPort = null;

        public MultiPulseValue(IEnumerable<T> value)
        {
            Value = value;
        }

        public INode SetPort(string portName, IConnection connection)
        {
            if (portName == "OUT" && connection is Connection<T> con) outputPort = con;
            return this;
        }

        public async Task ExecuteAsync()
        {
            foreach (var item in Value)
                outputPort?.Send(item);

            outputPort?.Close();
        }
    }
}
