using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class FilterNode<T> : INode
{
    private readonly Func<T, bool> _predicate;
    private Connection<T>? _inputPort  = null;
    private Connection<T>? _outputPort = null;

    public FilterNode(Func<T, bool> predicate) { _predicate = predicate; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input  && connection is Connection<T> con0) _inputPort  = con0;
        if (portName == Ports.Output && connection is Connection<T> con1) _outputPort = con1;
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
            if (_predicate(packet))
                _outputPort?.Send(packet);
        }

        _outputPort?.Close();
    }
}
