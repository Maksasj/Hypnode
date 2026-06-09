using Hypnode.Core;
using Hypnode.Core.Modules;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

// Emits its current value each tick, then latches the next incoming value.
// The one-tick lag is what makes feedback cycles in the graph schedulable.
[HypnodeNode("delay", "Emits seed on first tick, then echoes each input next tick")]
public class DelayNode : INode
{
    private HypnodeValue _current;
    private Connection<HypnodeValue>? _inputPort;
    private Connection<HypnodeValue>? _outputPort;

    public DelayNode(HypnodeValue initialValue) => _current = initialValue;

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
