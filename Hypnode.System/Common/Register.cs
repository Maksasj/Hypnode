using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class Register<T> : INode
{
    public const string Input = "IN";

    private T? _value;
    private Connection<T>? _inputPort = null;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Input && connection is Connection<T> con) _inputPort = con;
        return this;
    }

    public T? GetValue() => _value;

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            _value = _inputPort.Receive();
        }
    }
}
