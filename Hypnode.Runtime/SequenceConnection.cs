using Hypnode.Core;

namespace Hypnode.Runtime
{
    public class SequenceConnection<T> : Connection<T>
    {
        private readonly Queue<T> buffer = new();
        private bool closed = false;

        public override bool HasData => buffer.Count > 0;
        public override bool IsClosed => closed;

        public override T Receive()
        {
            if (buffer.Count == 0)
                throw new InvalidOperationException("No data available in connection");

            return buffer.Dequeue();
        }

        public override bool TryReceive(out T packet)
        {
            if (buffer.Count > 0)
            {
                packet = buffer.Dequeue();
                return true;
            }

            packet = default!;
            return false;
        }

        public override void Send(T packet)
        {
            if (closed)
                throw new InvalidOperationException("Cannot send to a closed connection");

            buffer.Enqueue(packet);
        }

        public override void Close() => closed = true;
    }
}
