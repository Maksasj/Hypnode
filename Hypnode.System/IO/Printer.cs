using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.IO;

public class Printer<T> : INode
{
    private Connection<T>? _inputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input && connection is Connection<T> conn) _inputPort = conn;
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            Console.WriteLine($"{_inputPort.Receive()}");
        }
    }
}
