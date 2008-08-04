namespace Uniframework.Collections
{

    /// <summary>
    /// <para>
    /// A data structure commonly used for routing and buffering work (or any objects) between different
    /// threads in the same process; a queue may also be used by a single thread as a way
    /// to organize pieces of work in a simple way.
    /// </para>
    /// <para>
    /// Two subclasses, DoubleEndedQueue and PriorityQueue, are provided (see their documentation
    /// for more details).  Since they share a common parent class, code receiving objects
    /// from a Queue does not need to know the actual implementation, except in the special
    /// case where an object must be removed from the front of a double-ended queue.  Either
    /// DoubleEndedQueue or PriorityQueue may be used to good effect as a simple single-ended queue
    /// by using the methods overridden from this class, Queue.
    /// </para>
    /// <para>
    /// The queue classes are provided in order to remedy the following failings of
    /// System.Collections.Queue:
    /// - lack of support for double-ended and priority queues, two commonly-needed types
    /// in programming many different types of systems
    /// - lack of support for waiting with timeout in case of nothing being immediately
    /// available in the queue; this leads to polling, with unavoidable loss averaging
    /// half the polling interval
    /// - slow performance when synchronized
    /// - poor naming (method names Add and Remove are more easily and intuitively understood than 
    /// Enqueue and Dequeue, leading to more readable code)
    /// </para>
    /// <para>
    /// All Draco.Common.Logging.Collections.Queue instances are synchronized.  This is in recognition
    /// of the main use of such in-memory structures in actual practice, which is to route
    /// objects between threads in the same process.  Multiple threads requires synchronization,
    /// and the approach in the System.Collections namespace (wrapping an unsynchronized object in 
    /// a synchronized wrapper) unnecessarily slows performance in this situation due to 
    /// unnecessary method calls.
    /// </para>
    /// </summary>
    public abstract class Queue
    {
        private static object[] EMPTY_OBJECT_ARRAY = { };
        /// <summary>
        /// 
        /// </summary>
        protected bool isOpen = true;
        /// <summary>
        /// 
        /// </summary>
        protected int count = 0;
        /// <summary>
        /// 
        /// </summary>
        protected bool isNullAllowed = false;
        /// <summary>
        /// 
        /// </summary>
        protected object syncObj = new object();

        /// <summary>
        /// Adds an element to the queue.  Implemented in subclasses
        /// </summary>
        /// <param name="_object">The element to add</param>
        public abstract void Add(object obj);

        /// <summary>
        /// Gets the next object from the queue without removing it.  Implemented in subclasses
        /// </summary>
        public abstract object Peek();

        /// <summary>
        /// Gets the next object from the queue without removing it.  If no object is available,
        /// waits up to the specified number of milliseconds, then returns null if no object is
        /// available.
        /// Implemented in subclasses
        /// </summary>
        public abstract object Peek(int millisecondsTimeout);

        /// <summary>
        /// Removes the next object from the queue.  Implemented in subclasses
        /// </summary>
        public abstract object Remove();

        /// <summary>
        /// Removes the next object from the queue.  If no object is available,
        /// waits up to the specified number of milliseconds, then returns null if no object is
        /// available.
        /// Implemented in subclasses
        /// </summary>
        public abstract object Remove(int millisecondsTimeout);

        /// <summary>
        /// Removes all objects from the queue and returns them in ascending order, with 
        /// the element at index zero being the one "next in line"
        /// </summary>
        /// <returns></returns>
        public virtual object[] RemoveAll()
        {
            lock (syncObj)
            {
                if (count == 0)
                {
                    return EMPTY_OBJECT_ARRAY;
                }
                else
                {
                    object[] elements = new object[count];
                    for (int x = 0; x < elements.Length; x++)
                    {
                        elements[x] = Remove();
                    }
                    return elements;
                }
            }
        }

        /// <summary>
        /// Releases waiting threads, immediately returning null values to them, but
        /// leaves the queue open
        /// </summary>
        public void ReleaseWaitingThreads()
        {
            lock (syncObj)
            {
                bool open = isOpen;
                Close();
                isOpen = open;
            }
        }

        /// <summary>
        /// Allows the queue to accept input.  Each Queue instance is open when
        /// it is created.
        /// </summary>
        public virtual void Open()
        {
            lock (syncObj)
            {
                isOpen = true;
            }
        }

        /// <summary>
        /// Renders the queue incapable of accepting further input; also releases all
        /// waiting threads, immediately returning them null values
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Indicates whether the queue can accept further input
        /// </summary>
        public bool IsOpen
        {
            get
            {
                lock (syncObj)
                {
                    return isOpen;
                }
            }
        }

        /// <summary>
        /// Indicates the number of elements currently held in the queue
        /// </summary>
        public int Count
        {
            get
            {
                lock (syncObj)
                {
                    return count;
                }
            }
        }

        /// <summary>
        /// Clears all elements from the queue
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Gets and sets whether null values can be added to the queue
        /// </summary>
        public bool IsNullAllowed
        {
            get
            {
                lock (syncObj)
                {
                    return isNullAllowed;
                }
            }
            set
            {
                lock (syncObj)
                {
                    isNullAllowed = value;
                }
            }
        }


    }

}
