using Hypnode.Core;
using System.Collections.Concurrent;

namespace Hypnode.Async
{
    public class AsyncConnection<T> : Connection<T>
    {
        public BlockingCollection<T> Buffer { get; }

        public AsyncConnection()
        {
            Buffer = new BlockingCollection<T>(1);
        }

        public override T Receive() => Buffer.Take();

        public override bool TryReceive(out T packet) => Buffer.TryTake(out packet!, Timeout.Infinite);

        public override void Send(T packet) => Buffer.Add(packet);

        public override void Close() => Buffer.CompleteAdding();
    }
}
