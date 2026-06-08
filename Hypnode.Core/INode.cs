using System.Collections;

namespace Hypnode.Core;

public interface INode
{
    public INode SetPort(string portName, IConnection connection);

    IEnumerator Execute();
}

public static class NodeExtensions
{
    public static bool TryAttach<T>(ref Connection<T>? conn, IConnection abs)
    {
        if (conn is not null)
            return false;

        if (abs is Connection<T> typedConn)
        {
            conn = typedConn;
            return true;
        }

        return false;
    }
}
