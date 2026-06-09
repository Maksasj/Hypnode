namespace Hypnode.Core;

public static class NodeExtensions
{
    public static bool TryAttach(ref Connection? conn, IConnection abs)
    {
        if (conn is not null)
            return false;

        if (abs is Connection typedConn)
        {
            conn = typedConn;
            return true;
        }

        return false;
    }
}
