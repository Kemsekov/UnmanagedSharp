using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnmanagedSharp
{
    /// <summary>
    /// Unmanaged double linked list implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public unsafe class LinkedList<T> : IDisposable, ICollection<T>
    where T : unmanaged
    {
        /// <summary>
        /// Element of the list.
        /// </summary>
        public unsafe struct Element
        {
            public T Value;
            public Element* Parent;
            public Element* Next;
            public Element(T value, Element* parent, Element* next)
            {
                Value = value;
                Parent = parent;
                Next = next;
            }
            public Element(T value){
                Value = value;
                Parent = null;
                Next = null;
            }
        }
        public Element* Head;
        public Element* Tail;
        public int Count;

        int ICollection<T>.Count => this.Count;

        public bool IsReadOnly => false;

        public LinkedList()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }
        public LinkedList(IEnumerable<T> collection)
        {
            Head = null;
            Tail = null;
            Count = 0;
            AddRange(collection);
        }
        public LinkedList(int capacity)
        {
            Head = null;
            Tail = null;
            Count = 0;
        }
        public void Add(T item){
            this.AddLast(item);
        }
        public void AddLast(T item)
        {
            Element* newElement = (Element*)Marshal.AllocHGlobal(sizeof(Element));
            newElement->Value = item;
            newElement->Parent = null;
            newElement->Next = null;
            if (Head == null)
            {
                Head = newElement;
                Tail = newElement;
            }
            else
            {
                Tail->Next = newElement;
                newElement->Parent = Tail;
                Tail = newElement;
            }
            Count++;
        }
        public void AddFirst(T item)
        {
            Element* newElement = (Element*)Marshal.AllocHGlobal(sizeof(Element));
            newElement->Value = item;
            newElement->Parent = null;
            newElement->Next = Head;
            if (Head != null)
                Head->Parent = newElement;
            Head = newElement;
            if (Tail == null)
                Tail = newElement;
            Count++;
        }
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Add(item);
            }
        }
        public void Clear()
        {
            Element* current = Head;
            while (current != null)
            {
                Element* next = current->Next;
                Marshal.FreeHGlobal((IntPtr)current);
                current = next;
            }
            Head = null;
            Tail = null;
            Count = 0;
        }

        void Iterate(Action<T> action)
        {
            Element* current = Head;
            while (current != null)
            {
                action(current->Value);
                current = current->Next;
            }
        }

        public bool Contains(T item)
        {
            Element* current = Head;
            while (current != null)
            {
                if (current->Value.Equals(item))
                {
                    return true;
                }
                current = current->Next;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("The number of elements in the source is greater than the available space from arrayIndex to the end of the destination array.");
            }
            Iterate(item => array[arrayIndex++] = item);
        }

        /// <summary>
        /// Removes the first occurrence of the specified value
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            Element* current = Head;
            while (current != null)
            {
                if (current->Value.Equals(item))
                {
                    if (current->Parent == null)
                    {
                        Head = current->Next;
                    }
                    else
                    {
                        current->Parent->Next = current->Next;
                    }
                    if (current->Next == null)
                    {
                        Tail = current->Parent;
                    }
                    else
                    {
                        current->Next->Parent = current->Parent;
                    }
                    Marshal.FreeHGlobal((IntPtr)current);
                    Count--;
                    return true;
                }
                current = current->Next;
            }
            return false;
        }

        /// <summary>
        /// Removes all elements that match specified predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool Remove(Predicate<T> match)
        {
            Element* current = Head;
            while (current != null)
            {
                if (match(current->Value))
                {
                    if (current->Parent == null)
                    {
                        Head = current->Next;
                    }
                    else
                    {
                        current->Parent->Next = current->Next;
                    }
                    if (current->Next == null)
                    {
                        Tail = current->Parent;
                    }
                    else
                    {
                        current->Next->Parent = current->Parent;
                    }
                    Marshal.FreeHGlobal((IntPtr)current);
                    Count--;
                    return true;
                }
                current = current->Next;
            }
            return false;
        }
        public void RemoveAt(int index){
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            Element* current = Head;
            int i = 0;
            while (i < index)
            {
                current = current->Next;
                i++;
            }
            if (current->Parent == null)
            {
                Head = current->Next;
            }
            else
            {
                current->Parent->Next = current->Next;
            }
            if (current->Next == null)
            {
                Tail = current->Parent;
            }
            else
            {
                current->Next->Parent = current->Parent;
            }
            Marshal.FreeHGlobal((IntPtr)current);
            Count--;
        }

        //because unsafe code does not work in GetEnumerator we need to use pointers wrapper (Pointer<T>) and define a bunch of methods

        Pointer<Element> _head() => new(Head);
        void _move_pointer(ref Pointer<Element> p){
            p.Value = p.Value->Next;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            var p = _head();
            while (!p.IsNull())
            {
                yield return p.Dereference().Value;
                _move_pointer(ref p);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public void Dispose()
        {
            Clear();            
        }
        ~LinkedList()=>Dispose();
    }
}