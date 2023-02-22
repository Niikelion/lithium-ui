using System.Collections.Generic;

namespace UI.Li.Internal
{
    public class RemapHelper<T>
    {
        public int LeapStart;
        public int Offset;
        private int pointer;
        private readonly Dictionary<T, int> entryMapping;
        private readonly int[] sizes;

        public RemapHelper(int offset, IReadOnlyList<(T entry, int size)> entries)
        {
            entryMapping = new();
            int n = 1;
            while (n < entries.Count)
                n *= 2;

            sizes = new int[n * 2 - 1];
            
            pointer = n - 1;
            
            for (int i = 0; i < n; ++i)
            {
                int index = n + i - 1;

                if (i >= entries.Count)
                {
                    sizes[index] = 0;
                    continue;
                }

                var entry = entries[i];
                
                sizes[index] = entry.size;
                if (!EqualityComparer<T>.Default.Equals(entry.entry, default))
                    entryMapping[entries[i].entry] = index;
            }

            while (n > 1)
            {
                n /= 2;
                for (int i = 0; i < n; ++i)
                {
                    int index = n + i - 1;
                    int left = index * 2 + 1;
                    int right = left + 1;

                    sizes[index] = sizes[left] + sizes[right];
                }
            }

            Offset = offset;
            LeapStart = -1;
        }

        public void RemoveFirst()
        {
            if (pointer >= sizes.Length)
                return;

            int index = pointer;
            int size = sizes[index];
            
            sizes[index] = 0;
            
            while (index > 0)
            {
                index = (index - 1) / 2;
                sizes[index] -= size;
            }

            Offset += size;
            ++pointer;
        }
        
        public (int start, int size) FindAndRemove(T entry)
        {
            if (!entryMapping.TryGetValue(entry, out int index))
                return (0, 0);

            int size = sizes[index];
            int start = 0;

            sizes[index] = 0;

            while (index > 0)
            {
                start += index % 2 == 0 ? sizes[index - 1] : 0;
                index = (index - 1) / 2;
                sizes[index] -= size;
            }

            while (pointer < sizes.Length && sizes[pointer] == 0)
                ++pointer;

            int offset = Offset;
            
            Offset += size;
            
            return (start + offset, size);
        }
    }
}