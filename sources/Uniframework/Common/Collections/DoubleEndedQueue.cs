namespace Uniframework.Collections
{

    using System;
    using System.Threading;

    /// <summary>
    /// <para>
    /// An implementation of Draco.Common.Logging.Collections.Queue that functions as a double-ended
    /// queue.  Objects can be added to, peeked at, and removed from the front or the 
    /// back of the queue, using methods AddFirst(), AddLast(), PeekFirst(), PeekLast(),
    /// RemoveFirst(), and RemoveLast().
    /// </para>
    /// <para>
    /// The methods overridden from parent class Queue-- Add(), Peek(), and Remove()-- 
    /// support the semantics of a single-ended queue, where objects are added at the 
    /// back and removed from the front.  Thus, Add() performs the same logic as AddLast(),
    /// Remove() the same as RemoveFirst(), etc.
    /// </para>
    /// <para>
    /// The underlying storage mechanism used is a circular array.
    /// Queue implementations, including this one, are always synchronized.
    /// </para>
    /// </summary>
    public sealed class DoubleEndedQueue : Uniframework.Collections.Queue
    {
        private const float GROWTH_FACTOR = 3.0F;
        private const int INITIAL_CAPACITY = 1000;

        private int peekWaitCount = 0;
        private WaitingThreadNode peekWaitHeader;
        private int removeWaitCount = 0;
        private WaitingThreadNode removeWaitHeader;
        private int waitNodeCacheCount = 0;
        private WaitingThreadNode waitNodeCacheHeader;

        private object[] elements = new object[INITIAL_CAPACITY];
        private int startIndex = 0;
        private int endIndex = 0;
        private int capacity = INITIAL_CAPACITY;

        /// <summary>
        /// Constructs a new DoubleEndedQueue instance
        /// </summary>
        public DoubleEndedQueue()
        {
            peekWaitHeader = new WaitingThreadNode();
            peekWaitHeader.next = peekWaitHeader;
            peekWaitHeader.previous = peekWaitHeader;

            removeWaitHeader = new WaitingThreadNode();
            removeWaitHeader.next = removeWaitHeader;
            removeWaitHeader.previous = removeWaitHeader;

            waitNodeCacheHeader = new WaitingThreadNode();
        }

        /// <summary>
        /// Removes all objects from the queue
        /// </summary>
        public override void Clear()
        {
            lock (syncObj)
            {
                Array.Clear(elements, 0, capacity);
                count = 0;
                startIndex = 0;
                endIndex = 0;
            }
        }

        /// <summary>
        /// Adds the specified object to the back of the queue
        /// </summary>
        /// <param name="_object">The object to add</param>
        public override void Add(object _object)
        {
            lock (syncObj)
            {
                if (!isOpen)
                {
                    throw new InvalidOperationException("This instance is closed, and cannot accept input");
                }
                else if ((!isNullAllowed) && (_object == null))
                {
                    throw new ArgumentNullException("This instance does not allow null input");
                }

                if (peekWaitCount > 0)
                {
                    lock (peekWaitHeader)
                    {
                        if (peekWaitCount > 0)
                        {
                            WaitingThreadNode node;
                            node = peekWaitHeader.next;
                            while (node != peekWaitHeader)
                            {
                                node.valueReturned = true;
                                node.returnValue = _object;
                                node.previous = null;
                                node = node.next;
                                node.previous.next = null;
                            }
                            peekWaitCount = 0;
                            peekWaitHeader.next = null;
                            Monitor.PulseAll(peekWaitHeader);
                        }
                    }
                }

                if (removeWaitCount > 0)
                {
                    lock (removeWaitHeader)
                    {
                        if (removeWaitCount > 0)
                        {
                            WaitingThreadNode node = removeWaitHeader.next;
                            removeWaitHeader.next = node.next;
                            removeWaitHeader.next.previous = removeWaitHeader;
                            node.next = null;
                            node.previous = null;
                            removeWaitCount--;

                            lock (node)
                            {
                                node.valueReturned = true;
                                node.returnValue = _object;
                                Monitor.Pulse(node);
                            }
                        }
                    }
                }
                else
                {
                    if (count == capacity)
                    {
                        IncreaseArraySize();
                    }
                    if (count > 0)
                    {
                        endIndex++;
                        if (endIndex == capacity)
                        {
                            endIndex = 0;
                        }
                    }
                    elements[endIndex] = _object;
                    count++;
                }
            }
        }

        /// <summary>
        /// Adds the specified object to the back of the queue
        /// </summary>
        /// <param name="_object">The object to add</param>
        public void AddLast(object _object)
        {
            lock (syncObj)
            {
                if (!isOpen)
                {
                    throw new InvalidOperationException("This instance is closed, and cannot accept input");
                }
                else if ((!isNullAllowed) && (_object == null))
                {
                    throw new ArgumentNullException("This instance does not allow null input");
                }

                if (peekWaitCount > 0)
                {
                    lock (peekWaitHeader)
                    {
                        if (peekWaitCount > 0)
                        {
                            WaitingThreadNode node;
                            node = peekWaitHeader.next;
                            while (node != peekWaitHeader)
                            {
                                node.valueReturned = true;
                                node.returnValue = _object;
                                node.previous = null;
                                node = node.next;
                                node.previous.next = null;
                            }
                            peekWaitCount = 0;
                            peekWaitHeader.next = null;
                            Monitor.PulseAll(peekWaitHeader);
                        }
                    }
                }

                if (removeWaitCount > 0)
                {
                    lock (removeWaitHeader)
                    {
                        if (removeWaitCount > 0)
                        {
                            WaitingThreadNode node = removeWaitHeader.next;
                            removeWaitHeader.next = node.next;
                            removeWaitHeader.next.previous = removeWaitHeader;
                            node.next = null;
                            node.previous = null;
                            removeWaitCount--;

                            lock (node)
                            {
                                node.valueReturned = true;
                                node.returnValue = _object;
                                Monitor.Pulse(node);
                            }
                        }
                    }
                }
                else
                {
                    if (count == capacity)
                    {
                        IncreaseArraySize();
                    }
                    if (count > 0)
                    {
                        endIndex++;
                        if (endIndex == capacity)
                        {
                            endIndex = 0;
                        }
                    }
                    elements[endIndex] = _object;
                    count++;
                }
            }
        }

        /// <summary>
        /// Adds the specified object to the front of the queue, so it will be 
        /// returned by the next call to Remove() or RemoveFirst()
        /// </summary>
        /// <param name="_object">The object to add</param>
        public void AddFirst(object _object)
        {
            lock (syncObj)
            {
                if (!isOpen)
                {
                    throw new InvalidOperationException("This instance is closed, and cannot accept input");
                }
                else if ((!isNullAllowed) && (_object == null))
                {
                    throw new ArgumentNullException("This instance does not allow null input");
                }

                if (peekWaitCount > 0)
                {
                    lock (peekWaitHeader)
                    {
                        if (peekWaitCount > 0)
                        {
                            WaitingThreadNode node;
                            node = peekWaitHeader.next;
                            while (node != peekWaitHeader)
                            {
                                node.valueReturned = true;
                                node.returnValue = _object;
                                node.previous = null;
                                node = node.next;
                                node.previous.next = null;
                            }
                            peekWaitCount = 0;
                            peekWaitHeader.next = null;
                            Monitor.PulseAll(peekWaitHeader);
                        }
                    }
                }

                if (removeWaitCount > 0)
                {
                    lock (removeWaitHeader)
                    {
                        if (removeWaitCount > 0)
                        {
                            WaitingThreadNode node = removeWaitHeader.next;
                            removeWaitHeader.next = node.next;
                            removeWaitHeader.next.previous = removeWaitHeader;
                            node.next = null;
                            node.previous = null;
                            removeWaitCount--;

                            lock (node)
                            {
                                node.valueReturned = true;
                                node.returnValue = _object;
                                Monitor.Pulse(node);
                            }
                        }
                    }
                }
                else
                {


                    if (count == capacity)
                    {
                        IncreaseArraySize();
                    }
                    if (count > 0)
                    {
                        startIndex--;
                        if (startIndex < 0)
                        {
                            startIndex = elements.Length - 1;
                        }
                    }
                    elements[startIndex] = _object;
                    count++;
                }
            }
        }


        private void IncreaseArraySize()
        {
            capacity = (int)(elements.Length * GROWTH_FACTOR);
            object[] newElements = new object[capacity];

            Array.Copy(elements, startIndex, newElements, 0, ((elements.Length - startIndex)));
            if (startIndex > 0)
            {
                Array.Copy(elements, 0, newElements, ((elements.Length - startIndex)), (startIndex));
            }

            startIndex = 0;
            endIndex = elements.Length - 1;
            elements = newElements;
        }

        /// <summary>
        /// Gets the object at the front of the queue without removing it, returning null if the queue is empty
        /// </summary>
        /// <returns>The object currently on the front of the queue</returns>
        public override object Peek()
        {
            lock (syncObj)
            {
                return elements[startIndex];
            }
        }

        /// <summary>
        /// Gets the object at the front of the queue without removing it, returning null if the queue is empty
        /// </summary>
        /// <returns>The object currently on the front of the queue</returns>
        public object PeekFirst()
        {
            lock (syncObj)
            {
                return elements[startIndex];
            }
        }

        /// <summary>
        /// Gets the object at the front of the queue without removing it.  If the queue
        /// is currently empty, waits the specified number of milliseconds for the next
        /// input to the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait</param>
        /// <returns>The object at the front of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public override object Peek(int _millisecondsTimeout)
        {
            return PeekFirst(_millisecondsTimeout);
        }

        /// <summary>
        /// Gets the object at the front of the queue without removing it.  If the queue
        /// is currently empty, waits the specified number of milliseconds for the next
        /// input to the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait</param>
        /// <returns>The object at the front of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public object PeekFirst(int _millisecondsTimeout)
        {
            lock (syncObj)
            {
                if (count > 0)
                {
                    return elements[startIndex];
                }
                if ((_millisecondsTimeout <= 0) || (!isOpen))
                {
                    return null;
                }
            }

            object returnValue = null;
            lock (peekWaitHeader)
            {
                peekWaitCount++;

                WaitingThreadNode node;

                if (waitNodeCacheCount > 0)
                {
                    node = waitNodeCacheHeader.next;
                    waitNodeCacheHeader.next = node.next;
                    waitNodeCacheCount--;
                }
                else
                {
                    node = new WaitingThreadNode();
                }

                peekWaitHeader.previous.next = node;
                node.previous = peekWaitHeader.previous;
                peekWaitHeader.previous = node;
                node.next = peekWaitHeader;

                try
                {
                    Monitor.Wait(peekWaitHeader, _millisecondsTimeout);
                }
                catch { }

                if (node.valueReturned)
                {
                    returnValue = node.returnValue;
                    node.returnValue = null;
                    node.valueReturned = false;
                }
                else
                {
                    node.previous.next = node.next;
                    node.next.previous = node.previous;
                    node.previous = null;
                    peekWaitCount--;
                }

                node.next = waitNodeCacheHeader.next;
                waitNodeCacheHeader.next = node;
                waitNodeCacheCount++;
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the object at the back of the queue without removing it, or null if the
        /// queue is empty
        /// </summary>
        /// <returns>The object at the back of the queue, or null if the queue is empty</returns>
        public object PeekLast()
        {
            lock (syncObj)
            {
                return elements[endIndex];
            }
        }

        /// <summary>
        /// Gets the object at the back of the queue without removing it.  If the queue
        /// is currently empty, waits the specified number of milliseconds for the next
        /// input to the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait</param>
        /// <returns>The object at the back of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public object PeekLast(int _millisecondsTimeout)
        {
            lock (syncObj)
            {
                if (count > 0)
                {
                    return elements[endIndex];
                }
                if ((_millisecondsTimeout <= 0) || (!isOpen))
                {
                    return null;
                }
            }

            object returnValue = null;
            lock (peekWaitHeader)
            {
                peekWaitCount++;

                WaitingThreadNode node;

                if (waitNodeCacheCount > 0)
                {
                    node = waitNodeCacheHeader.next;
                    waitNodeCacheHeader.next = node.next;
                    waitNodeCacheCount--;
                }
                else
                {
                    node = new WaitingThreadNode();
                }

                peekWaitHeader.previous.next = node;
                node.previous = peekWaitHeader.previous;
                peekWaitHeader.previous = node;
                node.next = peekWaitHeader;

                try
                {
                    Monitor.Wait(peekWaitHeader, _millisecondsTimeout);
                }
                catch { }

                if (node.valueReturned)
                {
                    returnValue = node.returnValue;
                    node.returnValue = null;
                    node.valueReturned = false;
                }
                else
                {
                    node.previous.next = node.next;
                    node.next.previous = node.previous;
                    node.previous = null;
                    peekWaitCount--;
                }

                node.next = waitNodeCacheHeader.next;
                waitNodeCacheHeader.next = node;
                waitNodeCacheCount++;
            }
            return returnValue;
        }

        /// <summary>
        /// Removes and returns the object at the front of the queue.  If the queue
        /// is currently empty, returns null
        /// </summary>
        /// <returns>The object at the front of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public override object Remove()
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    returnValue = elements[startIndex++];
                    if (startIndex == capacity)
                    {
                        startIndex = 0;
                    }
                    count--;
                    return returnValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes and returns the object at the front of the queue.  If the queue
        /// is currently empty, returns null
        /// </summary>
        /// <returns>The object at the front of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public object RemoveFirst()
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    returnValue = elements[startIndex++];
                    if (startIndex == capacity)
                    {
                        startIndex = 0;
                    }
                    count--;
                    return returnValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes and returns the object at the back of the queue.  If the queue
        /// is currently empty, returns null
        /// </summary>
        /// <returns>The object at the back of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public object RemoveLast()
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    returnValue = elements[endIndex--];
                    if (endIndex < 0)
                    {
                        endIndex = capacity - 1;
                    }
                    count--;
                    return returnValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes and returns the object at the front of the queue.  If the queue
        /// is currently empty, waits the specified number of milliseconds for the next
        /// input to the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait</param>
        /// <returns>The object at the front of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public override object Remove(int _millisecondsTimeout)
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    returnValue = elements[startIndex++];
                    if (startIndex == capacity)
                    {
                        startIndex = 0;
                    }
                    count--;
                    return returnValue;
                }

                if ((_millisecondsTimeout <= 0) || (!isOpen))
                {
                    return null;
                }
            }

            WaitingThreadNode node;

            lock (removeWaitHeader)
            {
                peekWaitCount++;


                if (waitNodeCacheCount > 0)
                {
                    node = waitNodeCacheHeader.next;
                    waitNodeCacheHeader.next = node.next;
                    waitNodeCacheCount--;
                }
                else
                {
                    node = new WaitingThreadNode();
                }

                removeWaitHeader.previous.next = node;
                node.previous = removeWaitHeader.previous;
                removeWaitHeader.previous = node;
                node.next = removeWaitHeader;

            }

            lock (node)
            {
                if (!node.valueReturned)
                {
                    try
                    {
                        Monitor.Wait(node, _millisecondsTimeout);
                    }
                    catch { }
                }
            }

            lock (removeWaitHeader)
            {
                if (node.valueReturned)
                {
                    returnValue = node.returnValue;
                    node.returnValue = null;
                    node.valueReturned = false;
                }
                else
                {
                    node.previous.next = node.next;
                    node.next.previous = node.previous;
                    node.previous = null;
                    peekWaitCount--;
                }

                node.next = waitNodeCacheHeader.next;
                waitNodeCacheHeader.next = node;
                waitNodeCacheCount++;

            }

            return returnValue;
        }

        /// <summary>
        /// Removes and returns the object at the front of the queue.  If the queue
        /// is currently empty, waits the specified number of milliseconds for the next
        /// input to the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait</param>
        /// <returns>The object at the front of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public object RemoveFirst(int _millisecondsTimeout)
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    returnValue = elements[startIndex++];
                    if (startIndex == capacity)
                    {
                        startIndex = 0;
                    }
                    count--;
                    return returnValue;
                }

                if ((_millisecondsTimeout <= 0) || (!isOpen))
                {
                    return null;
                }
            }

            WaitingThreadNode node;

            lock (removeWaitHeader)
            {
                peekWaitCount++;


                if (waitNodeCacheCount > 0)
                {
                    node = waitNodeCacheHeader.next;
                    waitNodeCacheHeader.next = node.next;
                    waitNodeCacheCount--;
                }
                else
                {
                    node = new WaitingThreadNode();
                }

                removeWaitHeader.previous.next = node;
                node.previous = removeWaitHeader.previous;
                removeWaitHeader.previous = node;
                node.next = removeWaitHeader;

            }

            lock (node)
            {
                if (!node.valueReturned)
                {
                    try
                    {
                        Monitor.Wait(node, _millisecondsTimeout);
                    }
                    catch { }
                }
            }

            lock (removeWaitHeader)
            {
                if (node.valueReturned)
                {
                    returnValue = node.returnValue;
                    node.returnValue = null;
                    node.valueReturned = false;
                }
                else
                {
                    node.previous.next = node.next;
                    node.next.previous = node.previous;
                    node.previous = null;
                    peekWaitCount--;
                }

                node.next = waitNodeCacheHeader.next;
                waitNodeCacheHeader.next = node;
                waitNodeCacheCount++;

            }

            return returnValue;
        }

        /// <summary>
        /// Removes and returns the object at the back of the queue.  If the queue
        /// is currently empty, waits the specified number of milliseconds for the next
        /// input to the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait</param>
        /// <returns>The object at the back of the queue, or null if the queue was initially empty and the timeout expired</returns>
        public object RemoveLast(int _millisecondsTimeout)
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    returnValue = elements[endIndex--];
                    if (endIndex < 0)
                    {
                        endIndex = capacity - 1;
                    }
                    count--;
                    return returnValue;
                }

                if ((_millisecondsTimeout <= 0) || (!isOpen))
                {
                    return null;
                }
            }

            WaitingThreadNode node;

            lock (removeWaitHeader)
            {
                peekWaitCount++;


                if (waitNodeCacheCount > 0)
                {
                    node = waitNodeCacheHeader.next;
                    waitNodeCacheHeader.next = node.next;
                    waitNodeCacheCount--;
                }
                else
                {
                    node = new WaitingThreadNode();
                }

                removeWaitHeader.previous.next = node;
                node.previous = removeWaitHeader.previous;
                removeWaitHeader.previous = node;
                node.next = removeWaitHeader;

            }

            lock (node)
            {
                if (!node.valueReturned)
                {
                    try
                    {
                        Monitor.Wait(node, _millisecondsTimeout);
                    }
                    catch { }
                }
            }

            lock (removeWaitHeader)
            {
                if (node.valueReturned)
                {
                    returnValue = node.returnValue;
                    node.returnValue = null;
                    node.valueReturned = false;
                }
                else
                {
                    node.previous.next = node.next;
                    node.next.previous = node.previous;
                    node.previous = null;
                    peekWaitCount--;
                }

                node.next = waitNodeCacheHeader.next;
                waitNodeCacheHeader.next = node;
                waitNodeCacheCount++;

            }

            return returnValue;

        }

        /// <summary>
        /// Renders the queue incapable of accepting further input, and immediately
        /// returns null to waiting threads
        /// </summary>
        public override void Close()
        {
            lock (syncObj)
            {
                if (!isOpen)
                {
                    return;
                }
                WaitingThreadNode node;

                if (peekWaitCount > 0)
                {
                    lock (peekWaitHeader)
                    {
                        if (peekWaitCount > 0)
                        {
                            node = peekWaitHeader.next;
                            while (node != peekWaitHeader)
                            {
                                node.valueReturned = true;
                                node.returnValue = null;
                                node.previous = null;
                                node = node.next;
                                node.previous.next = null;
                            }
                            peekWaitCount = 0;
                            peekWaitHeader.next = null;
                            Monitor.PulseAll(peekWaitHeader);
                        }
                    }
                }

                if (removeWaitCount > 0)
                {
                    lock (removeWaitHeader)
                    {
                        if (removeWaitCount > 0)
                        {
                            node = removeWaitHeader.next;
                            WaitingThreadNode[] nodes = new WaitingThreadNode[removeWaitCount];
                            for (int x = 0; x < removeWaitCount; x++)
                            {
                                nodes[x] = node;
                                node.valueReturned = true;
                                node.returnValue = null;
                                node.previous = null;
                                node = node.next;
                                node.previous.next = null;
                            }
                            for (int x = 0; x < removeWaitCount; x++)
                            {
                                lock (nodes[x])
                                {
                                    Monitor.Pulse(nodes[x]);
                                }
                            }
                            removeWaitHeader.next = removeWaitHeader;
                            removeWaitHeader.previous = removeWaitHeader;
                            removeWaitCount = 0;
                        }
                    }
                }

            }
        }

        private sealed class WaitingThreadNode
        {
            public object returnValue = null;
            public bool valueReturned = false;
            public WaitingThreadNode next;
            public WaitingThreadNode previous;

            public WaitingThreadNode() { }
        }


    }

}
