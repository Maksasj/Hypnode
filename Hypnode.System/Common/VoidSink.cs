using Hypnode.Core;
using Hypnode.Core.Types;
using System.Collections;

namespace Hypnode.System.Common;

public class VoidSink : INode
{
    public const string Input = "_";

    private readonly List<Connection<HypnodeValue>> _inputPorts = [];

    public INode SetPort(string portName, IConnection connection)
    {
        if (portName == Input && connection is Connection<HypnodeValue> conn)
            _inputPorts.Add(conn);
        return this;
    }

    public IEnumerator Execute()
    {
        bool anyReceived;
        do
        {
            anyReceived = false;
            foreach (var conn in _inputPorts)
                while (conn.TryReceive(out _)) anyReceived = true;

            if (anyReceived) yield return null;
        }
        while (anyReceived);
    }
}
