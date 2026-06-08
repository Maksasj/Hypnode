using Hypnode.Core;
using System.Collections;

namespace Hypnode.System.Common;

public class VoidSink<T> : INode
{
    private readonly List<Connection<T>> inputPorts = [];

    public INode SetPort(string portName, IConnection connection)
    {
        if (connection is Connection<T> conn) inputPorts.Add(conn);
        return this;
    }

    public IEnumerator Execute()
    {
        bool anyReceived;
        do
        {
            anyReceived = false;
            foreach (var conn in inputPorts)
            {
                while (conn.TryReceive(out _))
                    anyReceived = true;
            }

            if (anyReceived) yield return null;
        }
        while (anyReceived);
    }
}
