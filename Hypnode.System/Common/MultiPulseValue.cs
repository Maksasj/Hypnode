using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class MultiPulseValue : INode
{
    private readonly IEnumerable<HypnodeValue> _values;
    private Connection? _outputPort;

    public MultiPulseValue(IEnumerable<HypnodeValue> values) => _values = values;

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Ports.Output) NodeExtensions.TryAttach(ref _outputPort, connection);
        return this;
    }

    public IEnumerator Execute()
    {
        foreach (var item in _values)
            _outputPort?.Send(item);
        _outputPort?.Close();
        yield break;
    }
}
