namespace System.Collections
{
    /// <summary>
    /// Provides a wrapper for any object's instance to be represented as <see cref="ISeriesItem{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SeriesItem<T> : ISeriesItem<SeriesItem<T>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SeriesItem{T}"/> class.
        /// </summary>
        /// <param name="data">The wrapped object intance.</param>
        /// <param name="ordinal">This instance ordinal number.</param>
        public SeriesItem(T data, int ordinal)
        {
            Data = data;
            Ordinal = ordinal;
        }

        /// <summary>
        /// Gets the ordinal number of this instance in the series.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets the wrapped object's instance.
        /// </summary>
        public T Data { get; set; }
        
        int IComparable<SeriesItem<T>>.CompareTo(SeriesItem<T> other)
        {
            if (other.Ordinal == Ordinal) return 0;
            return Ordinal < other.Ordinal ? -1 : 1;
        }
    }
}
