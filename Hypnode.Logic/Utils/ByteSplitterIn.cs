using Hypnode.Core;
using System.Collections;

namespace Hypnode.Logic.Utils;

public class ByteSplitterIn : INode
{
    public const string Input = "IN";

    private Connection<byte>? _inputPort = null;
    private readonly Connection<LogicValue>[] _outputPorts = new Connection<LogicValue>[8];

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Input && connection is Connection<byte> connIn)
            _inputPort = connIn;

        if (int.TryParse(portName, out int idx) && idx >= 0 && idx < 8
            && connection is Connection<LogicValue> connBit)
            _outputPorts[idx] = connBit;

        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }

            var packet = _inputPort.Receive();
            LogicValue[] values = [.. Enumerable.Range(0, 8)
                .Select(i => (packet & (1 << i)) != 0)
                .Select(b => b ? LogicValue.True : LogicValue.False)];

            for (int i = 0; i < 8; ++i)
                _outputPorts[i]?.Send(values[i]);
        }

        foreach (var connection in _outputPorts)
            connection?.Close();
    }
}
