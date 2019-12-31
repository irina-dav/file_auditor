using System.Collections.Generic;

namespace Infrastructure
{
    public class CLimitedQueue<T> : Queue<T>
    {
        public int Limit { get; set; }

        public CLimitedQueue(int limit) : base(limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(item);
        }
    }
}
