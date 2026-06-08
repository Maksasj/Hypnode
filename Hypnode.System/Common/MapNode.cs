using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class MapNode<TIn, TOut> : INode
{
    private readonly Func<TIn, TOut> _map;
    private Connection<TIn>?  _inputPort  = null;
    private Connection<TOut>? _outputPort = null;

    public MapNode(Func<TIn, TOut> map) { _map = map; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input  && connection is Connection<TIn>  con0) _inputPort  = con0;
        if (portName == Ports.Output && connection is Connection<TOut> con1) _outputPort = con1;
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }
            _outputPort?.Send(_map(_inputPort.Receive()));
        }

        _outputPort?.Close();
    }
}
