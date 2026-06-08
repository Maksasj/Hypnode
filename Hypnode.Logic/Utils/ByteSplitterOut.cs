using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Utils;

public class ByteSplitterOut : INode
{
    private readonly Connection<LogicValue>[] _inputPorts = new Connection<LogicValue>[8];
    private Connection<byte>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == "0" && connection is Connection<LogicValue> conn0) _inputPorts[0] = conn0;
        if (portName == "1" && connection is Connection<LogicValue> conn1) _inputPorts[1] = conn1;
        if (portName == "2" && connection is Connection<LogicValue> conn2) _inputPorts[2] = conn2;
        if (portName == "3" && connection is Connection<LogicValue> conn3) _inputPorts[3] = conn3;
        if (portName == "4" && connection is Connection<LogicValue> conn4) _inputPorts[4] = conn4;
        if (portName == "5" && connection is Connection<LogicValue> conn5) _inputPorts[5] = conn5;
        if (portName == "6" && connection is Connection<LogicValue> conn6) _inputPorts[6] = conn6;
        if (portName == "7" && connection is Connection<LogicValue> conn7) _inputPorts[7] = conn7;
        if (portName == "OUT" && connection is Connection<byte> connOut) _outputPort = connOut;

        return this;
    }

    public IEnumerator Execute()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_inputPorts[i] is null)
                throw new InvalidOperationException($"Input port {i} is not set");
        }

        while (true)
        {
            bool anyExhausted = Enumerable.Range(0, 8).Any(i =>
                _inputPorts[i].IsClosed && !_inputPorts[i].HasData);

            if (anyExhausted)
                break;

            bool allReady = Enumerable.Range(0, 8).All(i => _inputPorts[i].HasData);

            if (!allReady)
            {
                yield return null;
                continue;
            }

            var values = new LogicValue[8];
            for (int i = 0; i < 8; i++)
                values[i] = _inputPorts[i].Receive();

            byte result = (byte)Enumerable.Range(0, 8)
                .Select(i => values[i] == LogicValue.True ? (1 << i) : 0)
                .Aggregate(0, (acc, bit) => acc | bit);

            _outputPort?.Send(result);
        }

        _outputPort?.Close();
    }
}
