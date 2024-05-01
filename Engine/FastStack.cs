using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    //https://github.com/Tearth/Cosette/blob/e433c8c9adfe92568f8585217a542db22688ad4b/Cosette/Engine/Common/FastStack.cs
    // Used for helping with reversing moves - old states are added to the stack
    public class FastStack<T>
    {
        private T[] _stack;
        private int _pointer;

        public FastStack(int capacity)
        {
            _stack = new T[capacity];
        }

        public void Push(T value)
        {
            _stack[_pointer++] = value;
        }

        public T Pop()
        {
            return _stack[--_pointer];
        }

        //public T Peek(int index)
        //{
        //    return _stack[_pointer - index - 1];
        //}

        public T Peek()
        {
            return _stack[_pointer - 1];
        }

        public int Count()
        {
            return _pointer;
        }

        public int CountValues(T value)
        {
            int count = 0;
            for(var i = 0; i < _pointer; i++)
            {
                if (value.Equals(_stack[i])) count++;
            }
            return count;
        }

        public void Clear()
        {
            _pointer = 0;
        }
    }
}
