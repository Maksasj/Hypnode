using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class Splitter : INode
{
    private Connection<HypnodeValue>? _inputPort;
    private readonly List<Connection<HypnodeValue>> _outputPorts = [];

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input) NodeExtensions.TryAttach(ref _inputPort, connection);
        if (portName == Ports.Output && connection is Connection<HypnodeValue> con) _outputPorts.Add(con);
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
            foreach (var conn in _outputPorts) conn.Send(packet);
        }

        foreach (var conn in _outputPorts) conn.Close();
    }
}
