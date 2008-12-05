// ***************************************************************
//  DisposableAndStartableBase.cs   version:  1.4    date: 06/10/2006
//  -------------------------------------------------------------
//	author:		Yangjun Deng
// 	email:		Midapexsoft@gmail.com
// 	purpose:	
//  -------------------------------------------------------------
//  Copyright (C) 2006 - Midapexsoft All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// Base class for disposable and startable objects.
    /// </summary>
    public abstract class DisposableAndStartableBase : StartableBase, IDisposable
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
        protected virtual void Free(bool disposing)
        {
            //-----
        }

        /// <summary>
        /// Free the object.
        /// </summary>
        public void Dispose()
        {
            lock (this) {
                if (disposed == false) {
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
        ~DisposableAndStartableBase()
        {
            Free(false);
        }

        #endregion
    }
}
