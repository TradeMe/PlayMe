using System;

namespace SpotiFire.SpotifyLib
{
    internal class DelegateList<T> : DelegateArray<T>, IEditableArray<T>
    {
        protected Action<T, int> addFunc;
        protected Action<int> removeFunc;
        protected Func<bool> readonlyFunc;
        public DelegateList(Func<int> getLength, Func<int, T> getIndex, Action<T, int> addFunc, Action<int> removeFunc, Func<bool> readonlyFunc)
            : base(getLength, getIndex)
        {
            this.addFunc = addFunc;
            this.removeFunc = removeFunc;
            this.readonlyFunc = readonlyFunc;
        }

        public void Add(T item)
        {
            addFunc(item, getLength());
        }

        public void Clear()
        {
            while (Count > 0)
                removeFunc(0);
        }

        public bool Contains(T item)
        {
            foreach (T itm in this)
                if (itm.Equals(item))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("Array to small");

            int i = arrayIndex;
            foreach (T item in this)
                array[i++] = item;
        }

        public bool IsReadOnly
        {
            get
            {
                return readonlyFunc();
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;
            removeFunc(index);
            return true;
        }

        public int IndexOf(T item)
        {
            bool found = false;
            int i = 0, size = getLength();
            while (!found && i < size)
                if (!this[i].Equals(item))
                    i++;
                else
                    found = true;

            if (!found)
                return -1;
            return i;
        }

        public override IArray<TResult> Cast<TResult>()
        {
            return new DelegateList<TResult>(getLength, (index) =>
            {
                object obj = getIndex(index);
                return (TResult)obj;
            }, (value, index) =>
            {
                throw new InvalidOperationException();
            }, removeFunc, readonlyFunc);
        }
    }
}
