using System;
using System.Collections;
using System.Collections.Generic;

namespace FrustumCullingSolution.Scripts.Collections
{
    public class CVector<T> : IEnumerable<T>
    {
        private readonly int _bufferSize;
        private T[] _items;
        private int _size;

        public int Count => _size;
        public bool IsReadOnly => false;
        public T[] Items => _items;

        public CVector(int bufferSize)
        {
            _bufferSize = bufferSize;
            _items = new T[bufferSize];
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
            onSwap(_items[index], index);
        }
        
        public void Clear()
        {
            _size = 0;
            _items = new T[_bufferSize];
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
            var newSize = _size + _bufferSize;
            var newArray = new T[newSize];
            Array.Copy(_items, newArray, _size);
        }
    }
}