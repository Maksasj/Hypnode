using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class IfNode : INode
{
    public const string Then = "THEN";
    public const string Else = "ELSE";

    private readonly Func<HypnodeValue, bool> _predicate;
    private Connection? _inputPort;
    private Connection? _thenPort;
    private Connection? _elsePort;

    public IfNode(Func<HypnodeValue, bool> predicate) => _predicate = predicate;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input) NodeExtensions.TryAttach(ref _inputPort, connection);
        if (portName == Then) NodeExtensions.TryAttach(ref _thenPort, connection);
        if (portName == Else) NodeExtensions.TryAttach(ref _elsePort, connection);
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
