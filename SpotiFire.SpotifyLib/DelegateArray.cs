using System;
using System.Collections.Generic;

namespace SpotiFire.SpotifyLib
{
    internal class DelegateArray<T> : IArray<T>
    {
        protected Func<int> getLength;
        protected Func<int, T> getIndex;
        public DelegateArray(Func<int> getLength, Func<int, T> getIndex)
        {
            this.getLength = getLength;
            this.getIndex = getIndex;
        }

        public int Count
        {
            get
            {
                return getLength();
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= getLength() || index < 0)
                    throw new IndexOutOfRangeException();

                return getIndex(index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DelegateEnumerator(getLength, getIndex);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual IArray<TResult> Cast<TResult>()
        {
            return new DelegateArray<TResult>(getLength, (index) =>
            {
                return (TResult)Convert.ChangeType(getIndex(index), typeof(TResult));
            });
        }

        public class DelegateEnumerator : IEnumerator<T>
        {
            private Func<int> getLength;
            private Func<int, T> getIndex;
            private T current;
            private int currentIndex;
            private int length;
            public DelegateEnumerator(Func<int> getLength, Func<int, T> getIndex)
            {
                this.getLength = getLength;
                this.getIndex = getIndex;
                Reset();
            }

            public T Current
            {
                get
                {
                    if (currentIndex == -1 || currentIndex == int.MaxValue)
                        throw new Exception("Before first element");

                    if (current == null)
                        current = getIndex(currentIndex);

                    return current;
                }
            }

            public void Dispose()
            {
                getLength = null;
                getIndex = null;
                current = default(T);
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                currentIndex++;
                current = default(T);
                if (currentIndex >= length)
                {
                    currentIndex = int.MaxValue;
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                currentIndex = -1;
                current = default(T);
                length = getLength();
            }
        }
    }
}
