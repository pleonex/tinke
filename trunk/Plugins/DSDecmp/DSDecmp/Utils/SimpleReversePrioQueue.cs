using System;
using System.Collections.Generic;
using System.Text;

namespace DSDecmp.Utils
{
    /// <summary>
    /// Very simplistic implementation of a priority queue that returns items with lowest priority first.
    /// This is not the most efficient implementation, but required the least work while using the classes
    /// from the .NET collections, and without requiring importing another dll or several more class files
    /// in order to make it work.
    /// </summary>
    /// <typeparam name="TPrio">The type of the priority values.</typeparam>
    /// <typeparam name="TValue">The type of item to put into the queue.</typeparam>
    public class SimpleReversedPrioQueue<TPrio, TValue>
    {
        private SortedDictionary<TPrio, LinkedList<TValue>> items;
        private int itemCount;

        public int Count { get { return this.itemCount; } }

        public SimpleReversedPrioQueue()
        {
            this.items = new SortedDictionary<TPrio, LinkedList<TValue>>();
            this.itemCount = 0;
        }

        public void Enqueue(TPrio priority, TValue value)
        {
            if (!this.items.ContainsKey(priority))
                this.items.Add(priority, new LinkedList<TValue>());
            this.items[priority].AddLast(value);
            this.itemCount++;
        }

        public TValue Peek(out TPrio priority)
        {
            if (this.itemCount == 0)
                throw new IndexOutOfRangeException();
            foreach (KeyValuePair<TPrio, LinkedList<TValue>> kvp in this.items)
            {
                priority = kvp.Key;
                return kvp.Value.First.Value;
            }
            throw new IndexOutOfRangeException();
        }

        public TValue Dequeue(out TPrio priority)
        {
            if (this.itemCount == 0)
                throw new IndexOutOfRangeException();
            LinkedList<TValue> lowestLL = null;
            priority = default(TPrio);
            foreach (KeyValuePair<TPrio, LinkedList<TValue>> kvp in this.items)
            {
                lowestLL = kvp.Value;
                priority = kvp.Key;
                break;
            }

            TValue returnValue = lowestLL.First.Value;
            lowestLL.RemoveFirst();
            // remove unused linked lists. priorities will only grow.
            if (lowestLL.Count == 0)
            {
                this.items.Remove(priority);
            }
            this.itemCount--;
            return returnValue;
        }
    }
}
