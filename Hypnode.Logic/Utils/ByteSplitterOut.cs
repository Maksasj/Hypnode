using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Utils;

public class ByteSplitterOut : INode
{
    private readonly Connection<LogicValue>[] _inputPorts = new Connection<LogicValue>[8];
    private Connection<byte>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output && connection is Connection<byte> connOut)
            _outputPort = connOut;

        if (int.TryParse(portName, out int idx) && idx >= 0 && idx < 8
            && connection is Connection<LogicValue> connBit)
            _inputPorts[idx] = connBit;

        return this;
    }

    public IEnumerator Execute()
    {
        for (int i = 0; i < 8; i++)
            if (_inputPorts[i] is null)
                throw new InvalidOperationException($"Input port {i} is not set");

        while (true)
        {
            if (Enumerable.Range(0, 8).Any(i => _inputPorts[i].IsClosed && !_inputPorts[i].HasData))
                break;

            if (!Enumerable.Range(0, 8).All(i => _inputPorts[i].HasData))
            {
                yield return null;
                continue;
            }

            var values = new LogicValue[8];
            for (int i = 0; i < 8; i++) values[i] = _inputPorts[i].Receive();

            byte result = (byte)Enumerable.Range(0, 8)
                .Select(i => values[i] == LogicValue.True ? (1 << i) : 0)
                .Aggregate(0, (acc, bit) => acc | bit);

            _outputPort?.Send(result);
        }

        _outputPort?.Close();
    }
}
