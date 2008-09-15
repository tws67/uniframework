using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Describes an edit handler.
    /// </summary>
    public interface IEditHandler
    {
        /// <summary>
        /// Occurs when the ui element is entered.
        /// </summary>
        event EventHandler Enter;

        /// <summary>
        /// Occurs when the input focus leaves the ui element.
        /// </summary>
        event EventHandler Leave;

        /// <summary>
        /// Gets a value indicating whether the user can undo the previous operation in the ui element.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Undoes the last operation in the ui element.
        /// </summary>
        void Undo();

        /// <summary>
        /// Gets a value indicating whether there are actions that have occured within the ui control
        /// that can be reapplied.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Reapplies the last operation that was undone in the ui element.
        /// </summary>
        void Redo();

        /// <summary>
        /// The cut method can be applied.
        /// </summary>
        bool CanCut { get; }

        /// <summary>
        /// Moves the current selection in the ui element to the Clipboard.
        /// </summary>
        void Cut();

        /// <summary>
        /// The copy method can be applied.
        /// </summary>
        bool CanCopy { get; }

        /// <summary>
        /// Copies the current selection in the ui element to the Clipboard.
        /// </summary>
        void Copy();

        /// <summary>
        /// The paste method can be applied.
        /// </summary>
        bool CanPaste { get; }

        /// <summary>
        /// Pastes the contents of the Clipboard into the ui element.
        /// </summary>
        void Paste();

        /// <summary>
        /// The delete method can be applied.
        /// </summary>
        bool CanDelete { get; }

        /// <summary>
        /// Deletes the current selection in the ui element.
        /// </summary>
        void Delete();

        /// <summary>
        /// The select all method can be applied.
        /// </summary>
        bool CanSelectAll { get; }

        /// <summary>
        /// Select all text and all items respectively.
        /// </summary>
        void SelectAll();

        /// <summary>
        /// Gets a value indicating whether this instance can filter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can filter; otherwise, <c>false</c>.
        /// </value>
        bool CanFilter { get; }

        /// <summary>
        /// Filters this instance.
        /// </summary>
        void Filter();

        /// <summary>
        /// Gets a value indicating whether this instance can search.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can search; otherwise, <c>false</c>.
        /// </value>
        bool CanSearch { get; }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        void Search();

        /// <summary>
        /// Gets a value indicating whether this instance can replace.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can replace; otherwise, <c>false</c>.
        /// </value>
        bool CanReplace { get; }

        /// <summary>
        /// Replaces this instance.
        /// </summary>
        void Replace();
    }
}
