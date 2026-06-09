using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class FilterNode : INode
{
    private readonly Func<HypnodeValue, bool> _predicate;
    private Connection<HypnodeValue>? _inputPort;
    private Connection<HypnodeValue>? _outputPort;

    public FilterNode(Func<HypnodeValue, bool> predicate) => _predicate = predicate;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Input) NodeExtensions.TryAttach(ref _inputPort, connection);
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
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
            if (_predicate(packet)) _outputPort?.Send(packet);
        }

        _outputPort?.Close();
    }
}
