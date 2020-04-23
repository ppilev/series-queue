using System.Collections.Generic;
using System.Linq;

namespace System.Collections
{
    /// <summary>
    /// Represents a prioritized collection of <see cref="ISeriesItem{T}"/> items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SeriesQueue<T> where T : ISeriesItem<T>
    {
        private PriorityQueue<T> priorityQueue;

        /// <summary>
        /// Creates a new instance of <see cref="SeriesQueue{T}"/>.
        /// </summary>
        /// <param name="items">The list of binary-tree ordered items usually retrieved as a result of <see cref="ToList"/> method.</param>
        /// <param name="nextOrdinal">Optionally provides the next ordinal the queue can dequeue.</param>
        /// <exception cref="InvalidOperationException">The <see cref="ISeriesItem{T}.Ordinal"/> of the element at head of the queue cannot be ahead of <paramref name="nextOrdinal"/> of the queue.</exception>
        public SeriesQueue(List<T> items = default, int? nextOrdinal = null)
        {
            priorityQueue = PriorityQueue<T>.Load(items ?? new List<T>());
            var queueHead = priorityQueue.Count == 0 ? 0 : priorityQueue.Peek().Ordinal;

            if (nextOrdinal.HasValue)
            {
                if (queueHead > nextOrdinal.Value) 
                { 
                    throw new InvalidOperationException($"The ordinal of the element at head of the queue '{queueHead}' cannot be ahead of next ordinal '{nextOrdinal}' of the queue."); 
                }

                queueHead = nextOrdinal.Value;
            }

            NextOrdinal = queueHead;
        }

        /// <summary>
        /// Gets the ordinal number of next <see cref="ISeriesItem{T}"/> that can be dequeued.
        /// </summary>
        public int NextOrdinal { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="SeriesQueue{T}"/>.
        /// </summary>
        public int Count => priorityQueue.Count;

        /// <summary>
        /// Adds an object to the <see cref="SeriesQueue{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="SeriesQueue{T}"/></param>
        public void Enqueue(T item)
        {
            priorityQueue.Enqueue(item);
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="SeriesQueue{T}"/>.
        /// </summary>
        /// <returns>The object that is removed from the beginning of the <see cref="SeriesQueue{T}"/>.</returns>
        public T Dequeue()
        {
            return DequeueItem();
        }

        /// <summary>
        /// Adds an object to the <see cref="SeriesQueue{T}"/> and then removes and returns all sequential objects where <see cref="ISeriesItem{T}.Ordinal"/> 
        /// of the element at the head of the queue matches with <see cref="NextOrdinal"/> of the queue.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="SeriesQueue{T}"/>.</param>
        /// <returns>All removed objects.</returns>
        public IEnumerable<T> AddAndDequeueAll(T item)
        {
            priorityQueue.Enqueue(item);
            return DequeueAll().ToList();
        }

        /// <summary>
        /// Removes and returns all sequential objects when <see cref="ISeriesItem{T}.Ordinal"/> of the element at the head of the queue 
        /// matches with <see cref="NextOrdinal"/> of the queue.
        /// </summary>
        /// <returns>All removed objects.</returns>
        public IEnumerable<T> DequeueAll()
        {
            while (priorityQueue.Count > 0 && priorityQueue.Peek().Ordinal == NextOrdinal)
            {
                yield return DequeueItem(false);
            }
        }

        /// <summary>
        /// Returns the list of queue items as a binary-tree ordered list.
        /// </summary>
        /// <returns>The list of queue items.</returns>
        public IList<T> ToList() => priorityQueue.ToList();
       
        private T DequeueItem(bool checkHead = true)
        {
            if (!checkHead || priorityQueue.Peek().Ordinal == NextOrdinal)
            {
                NextOrdinal++;
                return priorityQueue.Dequeue();
            }

            return default(T);
        }
    }
}
