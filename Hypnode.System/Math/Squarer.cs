using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Math;

public class Squarer : INode
{
    private Connection<int>? _inputPort = null;
    private Connection<int>? _outputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input  && connection is Connection<int> con0) _inputPort = con0;
        if (portName == Ports.Output && connection is Connection<int> con1) _outputPort = con1;
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
            _outputPort?.Send(packet * packet);
        }

        _outputPort?.Close();
    }
}
