
namespace SpotiFire.SpotifyLib
{
    public interface IEditableArray<T> : IArray<T>
    {
        int IndexOf(T item);
        void Add(T item);
        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        bool IsReadOnly { get; }
        bool Remove(T item);
    }
}
