using System;

namespace Uniframework
{

    /// <summary>
    /// Base class for disposable objects.
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {

        #region Fields

        private bool disposed = false;

        #endregion

        #region Methods

        /// <summary>
        /// This method is called when object is being disposed. Override this method to free resources.
        /// </summary>
        /// <param name="dispodedByUser">
        /// Indicates if was fired by user or GC.
        /// if disposedByUser = true you can dispose unmanaged and managed resources. if false, only unmanaged resources can be disposed.
        /// </param>
        protected virtual void Free(bool dispodedByUser)
        {
            //-----
        }

        /// <summary>
        /// Free the object.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (disposed == false)
                {
                    Free(true);
                    disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates is object is already disposed.
        /// </summary>
        protected bool Disposed
        {
            get
            {
                lock (this)
                {
                    return disposed;
                }
            }
        }

        #endregion

        #region Free

        /// <summary>
        /// Destructor. (Finalize)
        /// </summary>
        ~DisposableBase()
        {
            Free(false);
        }

        #endregion

    }

}