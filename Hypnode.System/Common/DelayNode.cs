using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

// Emits its current value each tick, then latches the next incoming value.
// The one-tick lag is what makes feedback cycles in the graph schedulable.
public class DelayNode<T> : INode
{
    private T _current;
    private Connection<T>? _inputPort = null;
    private Connection<T>? _outputPort = null;

    public DelayNode(T initialValue) => _current = initialValue;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input && connection is Connection<T> con0) _inputPort = con0;
        if (portName == Ports.Output && connection is Connection<T> con1) _outputPort = con1;
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            _outputPort?.Send(_current);

            while (!_inputPort.HasData)
            {
                if (_inputPort.IsClosed) { _outputPort?.Close(); yield break; }
                yield return null;
            }

            _current = _inputPort.Receive();
        }
    }
}
