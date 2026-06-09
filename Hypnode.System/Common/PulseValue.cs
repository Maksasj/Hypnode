using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class PulseValue : INode
{
    private readonly HypnodeValue _value;
    private Connection<HypnodeValue>? _outputPort;

    public PulseValue(HypnodeValue value) => _value = value;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        _outputPort?.Send(_value);
        _outputPort?.Close();
        yield break;
    }
}
