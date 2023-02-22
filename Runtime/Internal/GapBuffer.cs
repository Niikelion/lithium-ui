using System;
using UnityEngine;
using JetBrains.Annotations;

namespace UI.Li.Internal
{
    /// <summary>
    /// GapBuffer data structure implementation.
    /// </summary>
    /// <typeparam name="T">Type of elements stored</typeparam>
    public class GapBuffer<T>
    {
        /// <summary>
        /// Number of elements stored inside collection.
        /// </summary>
        [PublicAPI] public int Count => buffer.Length - GapSize;

        private int GapSize => gapEnd - gapStart;
        
        private T[] buffer;
        private int gapStart, gapEnd;
        private readonly int chunkSize;

        /// <summary>
        /// Constructs <see cref="GapBuffer{T}"/> using given chunk size. Underlying array length will be multiple of chunkSize.
        /// </summary>
        [PublicAPI] public GapBuffer(int chunkSize = 256)
        {
            this.chunkSize = chunkSize;
            buffer = new T[chunkSize];
            gapStart = 0;
            gapEnd = chunkSize;
        }

        /// <summary>
        /// Inserts value at given index.
        /// </summary>
        /// <remarks>When previous operation was near given index, complexity is near constant, linear otherwise.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than <c>0</c> or greater than <see cref="Count"/>.</exception>
        [PublicAPI] public void Insert(int index, T value)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            //if gap is at the end of the buffer it may be faster to allocate bigger buffer before moving gap
            if (gapEnd == buffer.Length)
            {
                EnsureGapSize(1);
                MoveGapTo(index);
            }
            else //otherwise we may be moving gap to the end, so move map first just in case
            {
                MoveGapTo(index);
                EnsureGapSize(1);
            }

            buffer[index] = value;
            ++gapStart;
        }

        /// <summary>
        /// Removes value at given index.
        /// </summary>
        /// <remarks>When previous operation was near given index, complexity is near constant, linear otherwise.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than <c>0</c> or greater than <see cref="Count"/>.</exception>
        [PublicAPI] public void RemoveAt(int index)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            MoveGapTo(index);
            buffer[gapEnd] = default;
            ++gapEnd;
        }

        /// <summary>
        /// Returns value at given index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than <c>0</c> or greater than <see cref="Count"/>.</exception>
        [PublicAPI] public T Get(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return index < gapStart ? buffer[index] : buffer[index + GapSize];
        }

        /// <summary>
        /// Updates value at given index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than <c>0</c> or greater than <see cref="Count"/>.</exception>
        [PublicAPI] public void Set(int index, T value)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index < gapStart)
                buffer[index] = value;
            else
                buffer[index + GapSize] = value;
        }

        /// <summary>
        /// Gets/sets value at given index.
        /// </summary>
        /// <seealso cref="Set"/>
        /// <seealso cref="Get"/>
        [PublicAPI] public T this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        private void EnsureGapSize(int size)
        {
            int gapSize = GapSize;

            if (gapSize >= size)
                return;

            int spaceNeeded = size - gapSize;
            int remainder = spaceNeeded % chunkSize;
            spaceNeeded += remainder > 0 ? chunkSize - remainder : 0;

            int oldBufferSize = buffer.Length;
            
            Array.Resize(ref buffer, oldBufferSize + spaceNeeded);

            int oldStart = gapStart, oldEnd = gapEnd;

            gapStart = oldBufferSize;
            gapEnd = buffer.Length;
            MoveGapTo(oldEnd);
            gapStart = oldStart;
        }
        
        private void MoveGapTo(int index)
        {
            if (index == gapStart)
                return;
            
            Debug.Assert(index >= 0 && index <= Count, "Trying to insert gap outside bounds of buffer");

            bool toLeft = index < gapStart;
            
            int count = toLeft ? gapStart - index : index - gapStart;

            int oldStart = gapStart, oldEnd = gapEnd;
            
            gapEnd = index + GapSize;
            gapStart = index;

            int clearStart, clearEnd;

            if (toLeft)
            {
                Array.Copy(buffer, index, buffer, oldEnd - count, count);
                clearStart = gapStart;
                clearEnd = Math.Min(oldStart, gapEnd);
            }
            else
            {
                Array.Copy(buffer, oldEnd, buffer, oldStart, count);
                clearStart = Math.Max(oldEnd, gapStart);
                clearEnd = gapEnd;
            }
            
            Array.Clear(buffer, clearStart, clearEnd - clearStart);
        }
    }
}