using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.IO;

public class Assert : INode
{
    private Connection<bool>? _inputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input && connection is Connection<bool> con) _inputPort = con;
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            if (!_inputPort.Receive()) throw new InvalidOperationException("Assertion failed");
        }
    }
}
