using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class IfNode<T> : INode
{
    public const string Then = "THEN";
    public const string Else = "ELSE";

    private readonly Func<T, bool> _predicate;
    private Connection<T>? _inputPort = null;
    private Connection<T>? _thenPort = null;
    private Connection<T>? _elsePort = null;

    public IfNode(Func<T, bool> predicate) { _predicate = predicate; }

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input && connection is Connection<T> con0) _inputPort = con0;
        if (portName == Then && connection is Connection<T> con1) _thenPort = con1;
        if (portName == Else && connection is Connection<T> con2) _elsePort = con2;
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
                _thenPort?.Send(packet);
            else
                _elsePort?.Send(packet);
        }

        _thenPort?.Close();
        _elsePort?.Close();
    }
}
