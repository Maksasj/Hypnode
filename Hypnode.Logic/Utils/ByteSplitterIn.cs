using Hypnode.Core;

namespace Hypnode.Logic.Utils
{
    public class ByteSplitterIn : INode
    {
        private Connection<byte>? inputPort = null;
        private readonly Connection<LogicValue>[] outputPorts;

        public ByteSplitterIn()
        {
            outputPorts = [];
            outputPorts = new Connection<LogicValue>[8];
        }

        public INode SetPort(string portName, IConnection connection)
        {
            if (portName == "IN" && connection is Connection<byte> connIn) inputPort = connIn;
            if (portName == "0" && connection is Connection<LogicValue> conn0) outputPorts[0] = conn0;
            if (portName == "1" && connection is Connection<LogicValue> conn1) outputPorts[1] = conn1;
            if (portName == "2" && connection is Connection<LogicValue> conn2) outputPorts[2] = conn2;
            if (portName == "3" && connection is Connection<LogicValue> conn3) outputPorts[3] = conn3;
            if (portName == "4" && connection is Connection<LogicValue> conn4) outputPorts[4] = conn4;
            if (portName == "5" && connection is Connection<LogicValue> conn5) outputPorts[5] = conn5;
            if (portName == "6" && connection is Connection<LogicValue> conn6) outputPorts[6] = conn6;
            if (portName == "7" && connection is Connection<LogicValue> conn7) outputPorts[7] = conn7;

            return this;
        }

        public async Task ExecuteAsync()
        {
            if (inputPort is null)
                throw new InvalidOperationException("Input port is not set");

            while (inputPort.TryReceive(out var packet))
            {
                LogicValue[] values = [.. Enumerable.Range(0, 8)
                        .Select(i => (packet & (1 << i)) != 0)
                        .Select(b => b ? LogicValue.True : LogicValue.False)];

                for (int i = 0; i < 8; ++i)
                    outputPorts[i]?.Send(values[i]);
            }

            foreach (var connection in outputPorts)
                connection?.Close();
        }
    }
}
