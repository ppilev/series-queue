namespace System.Collections
{
    /// <summary>
    /// Represents an item with a position in a series.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISeriesItem<in T> : IComparable<T> where T : IComparable<T>
    {
        /// <summary>
        /// Gets the position of an item in a series.
        /// </summary>
        int Ordinal { get; }
    }
}
