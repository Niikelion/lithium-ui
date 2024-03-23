using System;
using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace UI.Li
{
    [PublicAPI] public class MutableList<T> : IMutableValue, IList<T>
    {
        public int Count => values.Count;
        public bool IsReadOnly => false;
        public IEnumerable<(ulong id, T value)> IndexedValues
        {
            get
            {
                foreach (var item in values)
                    yield return (item.id, item.value);
            }
        }

        private readonly List<(ulong id, T value, IMutableValue mutable)> values = new();
        private ulong nextId;

        public event Action OnValueChanged;

        public MutableList() { }

        public MutableList(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                InternalAdd(element);
        }

        public void Dispose()
        {
            Clear();
            nextId = 0;
            OnValueChanged = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in values)
                yield return item.value;
        }

        public T this[int index]
        {
            get => values[index].value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(values[index].value, value))
                    return;

                if (values[index].mutable != null)
                    values[index].mutable.OnValueChanged -= BroadcastUpdate;
                
                values[index] = CreateEntry(value);

                BroadcastUpdate();
            }
        }

        public void Swap(int index1, int index2)
        {
            if (index1 < 0 || index1 >= Count)
                throw new ArgumentOutOfRangeException(nameof(index1));
            if (index2 < 0 || index2 >= Count)
                throw new ArgumentOutOfRangeException(nameof(index2));

            (values[index1], values[index2]) = (values[index2], values[index1]);
            BroadcastUpdate();
        }

        public void Add(T item)
        {
            InternalAdd(item);
            BroadcastUpdate();
        }

        public void Clear()
        {
            for (int i=0; i<values.Count; ++i)
                UnregisterValue(i);

            values.Clear();
            BroadcastUpdate();
        }

        public bool Contains(T item)
        {
            foreach (var i in values)
                if (EqualityComparer<T>.Default.Equals(i.value, item))
                    return true;

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int end = Math.Min(array.Length, arrayIndex + Count);

            for (int i = arrayIndex; i < end; ++i)
                array[i] = values[i].value;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;
            
            RemoveAt(index);
            return true;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < values.Count; ++i)
                if (EqualityComparer<T>.Default.Equals(values[i].value, item))
                    return i;

            return -1;
        }

        public void Insert(int index, T item)
        {
            values.Insert(index, CreateEntry(item));
            BroadcastUpdate();
        }

        public void RemoveAt(int index)
        {
            UnregisterValue(index);
            values.RemoveAt(index);
            BroadcastUpdate();
        }

        public override string ToString() => $"[ {string.Join(", ", values.Select(v => v.value))} ]";

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private ulong GetNextId()
        {
            ulong ret = nextId++;

            if (nextId == 0)
                throw new Exception("Index overflow, not sure how but yeah");

            return ret;
        }

        private void InternalAdd(T item) => values.Add(CreateEntry(item));

        private (ulong id, T value, IMutableValue mutable) CreateEntry(T item)
        {
            if (item is IMutableValue mutable)
                mutable.OnValueChanged += BroadcastUpdate;
            else
                mutable = null;
            
            return (GetNextId(), item, mutable);
        }

        private void BroadcastUpdate() => OnValueChanged?.Invoke();

        private void UnregisterValue(int index)
        {
            if (values[index].mutable != null)
                values[index].mutable.OnValueChanged -= BroadcastUpdate;
        }
    }
}