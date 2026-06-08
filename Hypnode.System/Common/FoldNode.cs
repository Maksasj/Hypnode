using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class FoldNode<TIn, TOut> : INode
{
    private readonly TOut _seed;
    private readonly Func<TOut, TIn, TOut> _folder;
    private Connection<TIn>? _inputPort = null;
    private Connection<TOut>? _outputPort = null;

    public FoldNode(TOut seed, Func<TOut, TIn, TOut> folder)
    {
        _seed = seed;
        _folder = folder;
    }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input && connection is Connection<TIn> con0) _inputPort = con0;
        if (portName == Ports.Output && connection is Connection<TOut> con1) _outputPort = con1;
        return this;
    }

    public IEnumerator Execute()
    {
        if (_inputPort is null) throw new InvalidOperationException("Input port is not set");

        var acc = _seed;

        while (true)
        {
            if (_inputPort.IsClosed && !_inputPort.HasData) break;
            if (!_inputPort.HasData) { yield return null; continue; }

            acc = _folder(acc, _inputPort.Receive());
            _outputPort?.Send(acc);
        }

        _outputPort?.Close();
    }
}
