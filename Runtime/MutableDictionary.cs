using System;
using System.Linq;
using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace UI.Li
{
    [PublicAPI] public class MutableDictionary<TKey, TValue>: IMutableValue, IDictionary<TKey, TValue>
    {
        public ICollection<TKey> Keys => values.Keys;
        public ICollection<TValue> Values => values.Select(v => v.Value.value).ToArray();

        public int Count => values.Count;
        public bool IsReadOnly => false;
        
        public event Action OnValueChanged;

        public MutableDictionary() { }

        public MutableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var item in dictionary)
                Add(item.Key, item.Value);
        }

        public TValue this[TKey key]
        {
            get => values[key].value;
            set
            {
                UnregisterValue(values[key].mutable);
                values[key] = CreateEntry(value);
                BroadcastUpdate();
            }
        }

        private readonly Dictionary<TKey, (TValue value, IMutableValue mutable)> values = new ();

        public void Dispose()
        {
            Clear();
            BroadcastUpdate();
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in values)
                yield return new(item.Key, item.Value.value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<TKey, TValue> item) =>
            values.Add(item.Key, CreateEntry(item.Value));

        public void Clear()
        {
            foreach (var item in values)
                UnregisterValue(item.Value.mutable);
            
            values.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            values.TryGetValue(item.Key, out var pair) && EqualityComparer<TValue>.Default.Equals(pair.value, item.Value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            values.Select(p => new KeyValuePair<TKey,TValue>(p.Key, p.Value.value)).ToArray().CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Contains(item))
                return false;

            if (!EqualityComparer<TValue>.Default.Equals(item.Value, values[item.Key].value))
                return false;
            
            return Remove(item.Key);
        }

        public void Add(TKey key, TValue value) => values.Add(key, CreateEntry(value));

        public bool ContainsKey(TKey key) => values.ContainsKey(key);

        public bool Remove(TKey key)
        {
            UnregisterValue(values.GetValueOrDefault(key, default).mutable);
            return values.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (values.TryGetValue(key, out var entry))
            {
                value = entry.value;
                return true;
            }

            value = default;
            
            return false;
        }

        private (TValue value, IMutableValue mutable) CreateEntry(TValue item)
        {
            if (item is IMutableValue mutable)
                mutable.OnValueChanged += BroadcastUpdate;
            else
                mutable = null;
            
            return (item, mutable);
        }

        private void UnregisterValue(IMutableValue mutable)
        {
            if (mutable != null)
                mutable.OnValueChanged -= BroadcastUpdate;
        }

        private void BroadcastUpdate() => OnValueChanged?.Invoke();
    }
}