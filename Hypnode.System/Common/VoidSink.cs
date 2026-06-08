using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class VoidSink<T> : INode
{
    public const string Input = "_";

    private readonly List<Connection<T>> _inputPorts = [];

    public INode SetPort(string portName, IConnection connection)
    {
        if (connection is Connection<T> conn) _inputPorts.Add(conn);
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
