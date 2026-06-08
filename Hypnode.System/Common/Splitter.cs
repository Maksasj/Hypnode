using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class Splitter<T> : INode
{
    public const string Input = "IN";
    public const string Output = "OUT";

    private Connection<T>? _inputPort = null;
    private readonly List<Connection<T>> _outputPorts = [];

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Input && connection is Connection<T> con0) _inputPort = con0;
        if (portName == Output && connection is Connection<T> con1) _outputPorts.Add(con1);
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
            foreach (var conn in _outputPorts)
                conn.Send(packet);
        }

        foreach (var conn in _outputPorts)
            conn.Close();
    }
}
