namespace Uniframework.Collections
{

    using System;
    using System.Collections;
    using System.Threading;

    /// <summary>
    /// <para>
    /// An implementation of Draco.Common.Logging.Collections.Queue that supports
    /// adding, peeking at, and removing objects with priority levels (expressed as int values).
    /// </para>
    /// <para>The predefined levels (High, MediumHigh, Medium, MediumLow, Low) are
    /// provided as a convenience, for code that needs only a few simple priority levels.
    /// </para>
    /// <para>
    /// Objects given higher priorities are returned first.  Using priority numbers from 0
    /// to 20 (which includes the predefined levels) will result in best performance, but any non-negative int may be used as
    /// a priority.
    /// </para>
    /// </summary>
    public sealed class PriorityQueue : Uniframework.Collections.Queue
    {

        /// <summary>
        /// A predefined priority corresponding to the number 20
        /// </summary>
        public const int High = 20;

        /// <summary>
        /// A predefined priority corresponding to the number 15
        /// </summary>
        public const int MediumHigh = 15;

        /// <summary>
        /// A predefined priority corresponding to the number 10
        /// </summary>
        public const int Medium = 10;

        /// <summary>
        /// A predefined priority corresponding to the number 5
        /// </summary>
        public const int MediumLow = 5;

        /// <summary>
        /// A predefined priority corresponding to the number 0
        /// </summary>
        public const int Low = 0;

        private int peekWaitCount = 0;
        private WaitingThreadNode peekWaitHeader;
        private int removeWaitCount = 0;
        private WaitingThreadNode removeWaitHeader;
        private int waitNodeCacheCount = 0;
        private WaitingThreadNode waitNodeCacheHeader;

        private Sublist[] sublistArray = new Sublist[51];
        private Hashtable sublistMap = new Hashtable(1000, 0.5F);
        private int sublistCount = 0;
        private Sublist highestPrioritySublist = null;


        private int defaultPriority = 0;
        private Sublist defaultPrioritySublist = null;

        /// <summary>
        /// Constructs a new PriorityQueue instance.
        /// </summary>
        public PriorityQueue()
        {
            peekWaitHeader = new WaitingThreadNode();
            peekWaitHeader.next = peekWaitHeader;
            peekWaitHeader.previous = peekWaitHeader;

            removeWaitHeader = new WaitingThreadNode();
            removeWaitHeader.next = removeWaitHeader;
            removeWaitHeader.previous = removeWaitHeader;

            waitNodeCacheHeader = new WaitingThreadNode();

            Sublist sublist;
            for (int x = 0; x <= 20; x++)
            {
                sublist = new Sublist();
                sublist.priority = x;
                sublistArray[x] = sublist;
                sublistMap.Add(x, sublist);
            }
            sublistCount = 21;
            highestPrioritySublist = null;
            defaultPrioritySublist = sublistArray[0];
        }

        /// <summary>
        /// Gets and sets the priority used when the Add() method is called without 
        /// a priority level.  This is required to support the Queue interface, and also
        /// allows use of a priority queue by code that knows nothing of priority levels
        /// </summary>
        public int DefaultPriority
        {
            get
            {
                lock (syncObj)
                {
                    return defaultPriority;
                }
            }
            set
            {
                lock (syncObj)
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Negative priorities are not allowed");
                    }
                    defaultPriority = value;
                    if (defaultPriority > 20)
                    {
                        defaultPrioritySublist = GetSublist(defaultPriority);
                    }
                    else
                    {
                        defaultPrioritySublist = sublistArray[defaultPriority];
                    }
                }
            }
        }

        private Sublist GetSublist(int priority)
        {
            Sublist sublist = (Sublist)sublistMap[priority];
            if (sublist == null)
            {
                sublist = new Sublist();
                sublist.priority = priority;
                sublistMap.Add(priority, sublist);
                if (sublistCount == sublistArray.Length)
                {
                    Sublist[] newSublistArray = new Sublist[sublistArray.Length * 2];
                    Array.Copy(sublistArray, 0, newSublistArray, 0, sublistArray.Length);
                    sublistArray = newSublistArray;
                }
                sublistArray[sublistCount++] = sublist;
                Array.Sort(sublistArray, 0, sublistCount);
            }
            return sublist;
        }

        /// <summary>
        /// Removes all objects from the queue
        /// </summary>
        public override void Clear()
        {
            lock (syncObj)
            {
                if (count > 0)
                {
                    Sublist sublist;
                    for (int x = 0; x < sublistCount; x++)
                    {
                        sublist = sublistArray[x];
                        if (sublist.count > 0)
                        {
                            count -= sublist.count;
                            sublist.Clear();
                            if (count == 0)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified object to the queue with the default priority
        /// (see the DefaultPriority property)
        /// </summary>
        /// <param name="_object">The object to add to the queue</param>
        public override void Add(object obj)
        {
            lock (syncObj)
            {
                if (!isOpen)
                {
                    throw new InvalidOperationException("This instance is closed, and cannot accept input");
                }
                else if ((!isNullAllowed) && (obj == null))
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
                                node.returnValue = obj;
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
                                node.returnValue = obj;
                                Monitor.Pulse(node);
                            }
                        }
                    }
                }
                else
                {
                    defaultPrioritySublist.AddLast(obj);
                    if ((highestPrioritySublist == null) || (highestPrioritySublist.priority < defaultPriority))
                    {
                        highestPrioritySublist = defaultPrioritySublist;
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// Adds the object argument to the queue with the specified priority
        /// </summary>
        /// <param name="_object">The object to add to the queue</param>
        /// <param name="_priority">The priority to attach to the object (the higher the priority, the earlier it will be returned from the queue)</param>
        public void Add(object obj, int priority)
        {
            if (priority < 0)
            {
                throw new ArgumentException("Negative priorities are not allowed");
            }

            lock (syncObj)
            {
                if (!isOpen)
                {
                    throw new InvalidOperationException("This instance is closed, and cannot accept input");
                }
                else if ((!isNullAllowed) && (obj == null))
                {
                    throw new ArgumentNullException("This instance does not allow null input");
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
                                node.returnValue = obj;
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
                            removeWaitHeader.next = node.next;
                            removeWaitHeader.next.previous = removeWaitHeader;
                            node.next = null;
                            node.previous = null;
                            removeWaitCount--;

                            lock (node)
                            {
                                node.valueReturned = true;
                                node.returnValue = obj;
                                Monitor.Pulse(node);
                            }
                        }
                    }
                }
                else
                {
                    Sublist sublist;
                    if (priority <= 20)
                    {
                        sublist = sublistArray[priority];
                    }
                    else
                    {
                        sublist = GetSublist(priority);
                    }
                    sublist.AddLast(obj);
                    if ((highestPrioritySublist == null) || (highestPrioritySublist.priority < priority))
                    {
                        highestPrioritySublist = sublist;
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// Gets the next (highest-priority) object without removing it from the queue
        /// </summary>
        /// <returns>The highest-priority object on the queue, or null if the queue is empty</returns>
        public override object Peek()
        {
            lock (syncObj)
            {
                if (count > 0)
                {
                    if (highestPrioritySublist != null)
                    {
                        return highestPrioritySublist.elements[highestPrioritySublist.startIndex];
                    }
                    else
                    {
                        for (int x = (sublistCount - 1); x > 0; x--)
                        {
                            if (sublistArray[x].count > 0)
                            {
                                highestPrioritySublist = sublistArray[x];
                                return highestPrioritySublist.elements[highestPrioritySublist.startIndex];
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the next (highest-priority) object without removing it from the queue
        /// </summary>
        /// <param name="_millisecondsTimeout">The number of milliseconds to wait for input if the queue is initially empty</param>
        /// <returns>The highest-priority object on the queue, or null if the queue is initially empty and the timeout expires</returns>
        public override object Peek(int millisecondsTimeout)
        {
            lock (syncObj)
            {
                if (count > 0)
                {
                    if (highestPrioritySublist != null)
                    {
                        return highestPrioritySublist.elements[highestPrioritySublist.startIndex];
                    }
                    else
                    {
                        for (int x = (sublistCount - 1); x > 0; x--)
                        {
                            if (sublistArray[x].count > 0)
                            {
                                highestPrioritySublist = sublistArray[x];
                                return highestPrioritySublist.elements[highestPrioritySublist.startIndex];
                            }
                        }
                    }
                }

                if ((millisecondsTimeout <= 0) || (!isOpen))
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
                    Monitor.Wait(peekWaitHeader, millisecondsTimeout);
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
        /// Removes the next (highest-priority) object from the queue
        /// </summary>
        /// <returns>The next object from the highest priority currently contained by the queue</returns>
        public override object Remove()
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    if (highestPrioritySublist != null)
                    {
                        returnValue = highestPrioritySublist.RemoveFirst();
                        if (highestPrioritySublist.count == 0)
                        {
                            highestPrioritySublist = null;
                        }
                        count--;
                        return returnValue;
                    }
                    else
                    {
                        Sublist sublist;
                        for (int x = (sublistCount - 1); x >= 0; x--)
                        {
                            sublist = sublistArray[x];
                            if (sublist.count > 0)
                            {
                                returnValue = sublist.RemoveFirst();
                                if (sublist.count > 0)
                                {
                                    highestPrioritySublist = sublist;
                                }
                                count--;
                                return returnValue;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Removes the next (highest-priority) object from the queue, or null if
        /// the queue is initially empty and the specified timeout is reached without
        /// input
        /// </summary>
        /// <param name="_millisecondsTimeout"></param>
        /// <returns>The next object from the highest priority currently contained by the queue</returns>
        public override object Remove(int millisecondsTimeout)
        {
            object returnValue = null;
            lock (syncObj)
            {
                if (count > 0)
                {
                    if (highestPrioritySublist != null)
                    {
                        returnValue = highestPrioritySublist.RemoveFirst();
                        if (highestPrioritySublist.count == 0)
                        {
                            highestPrioritySublist = null;
                        }
                        count--;
                        return returnValue;
                    }
                    else
                    {
                        Sublist sublist;
                        for (int x = (sublistCount - 1); x >= 0; x--)
                        {
                            sublist = sublistArray[x];
                            if (sublist.count > 0)
                            {
                                returnValue = sublist.RemoveFirst();
                                if (sublist.count > 0)
                                {
                                    highestPrioritySublist = sublist;
                                }
                                count--;
                                return returnValue;
                            }
                        }
                    }
                }

                if ((millisecondsTimeout <= 0) || (!isOpen))
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
                        Monitor.Wait(node, millisecondsTimeout);
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
        /// Renders this Queue incapable of accepting further input, and immediately
        /// returns null to any waiting threads
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

        private sealed class Sublist : IComparable
        {
            private const int INITIAL_CAPACITY = 50;
            public int priority;
            public int count = 0;
            public object[] elements = new object[INITIAL_CAPACITY];
            public int capacity = INITIAL_CAPACITY;
            public int startIndex = 0;
            public int endIndex = 0;

            public Sublist() { }

            public void Clear()
            {
                if (count > 0)
                {
                    Array.Clear(elements, 0, capacity);
                    count = 0;
                    startIndex = 0;
                    endIndex = 0;
                }
            }

            public void AddLast(object _object)
            {
                if (count == capacity)
                {
                    if (capacity == INITIAL_CAPACITY)
                    {
                        capacity *= 5;
                    }
                    else
                    {
                        capacity *= 2;
                    }
                    object[] newElements = new object[capacity];
                    Array.Copy(elements, startIndex, newElements, 0, (elements.Length - startIndex));
                    if (startIndex > 0)
                    {
                        Array.Copy(elements, 0, newElements, (elements.Length - startIndex), startIndex);
                    }

                    elements = newElements;
                    startIndex = 0;
                    endIndex = count - 1;
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

            public object RemoveFirst()
            {
                object element = elements[startIndex++];
                if (startIndex == capacity)
                {
                    startIndex = 0;
                }
                count--;
                return element;
            }

            /*
            public object GetFirst() {
                return elements[startIndex];
            }
            */

            public int CompareTo(object _object)
            {
                return (priority - ((Sublist)_object).priority);
            }
        }
    }
}
