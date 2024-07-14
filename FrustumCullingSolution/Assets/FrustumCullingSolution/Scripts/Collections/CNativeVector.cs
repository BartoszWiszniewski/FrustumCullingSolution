using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace FrustumCullingSolution.Scripts.Collections
{
    public class CNativeVector<T> : IEnumerable<T>, IDisposable where T : struct
    {
        private readonly Allocator _allocator;
        private readonly int _bufferSize;
        private NativeArray<T> _items;
        private int _size;

        public int Count => _size;
        public bool IsReadOnly => false;
        public bool IsCreated => _items.IsCreated;
        public NativeArray<T> Items => _items;

        public CNativeVector(int bufferSize, Allocator allocator)
        {
            _allocator = allocator;
            _bufferSize = bufferSize;
            _items = new NativeArray<T>(bufferSize, allocator);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < _size; index++)
            {
                yield return _items[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var index = 0; index < _size; index++)
            {
                yield return _items[index];
            }
        }

        public void Add(T item, Action<T, int> onAdd)
        {
            if (_size >= _items.Length)
            {
                Resize();
            }

            _items[_size] = item;
            onAdd?.Invoke(item, _size);
            _size++;
        }

        public void RemoveAt(int index, Action<T, int> onSwap)
        {
            _size--;
            if (_size <= index)
            {
                if (_size < 0)
                {
                    _size = 0;
                }
                return;
            }
            
            _items[index] = _items[_size];
            onSwap?.Invoke(_items[index], index);
            _items[_size] = default;
        }
        
        public void Clear()
        {
            _size = 0;
            _items.Dispose();
            _items = new NativeArray<T>(_bufferSize, _allocator);
        }
        
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    return default;
                }

                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                {
                    return;
                }

                _items[index] = value;
            }
        }

        private void Resize()
        {
            var newBufferSize = _size + _bufferSize;
            var newArray = new NativeArray<T>(newBufferSize, Allocator.Persistent);
            NativeArray<T>.Copy(_items, newArray, _size);
            _items.Dispose();
            _items = newArray;
        }

        public void Dispose()
        {
            _size = 0;
            _items.Dispose();
        }
    }
}