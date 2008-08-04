using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 序号构成项目
    /// </summary>
    [Serializable]
    public class SequenceItem
    {
        private byte seqno;
        private string name;
        private string variable;
        private int increment;
        private int length;
        private char padChar = '0';
        private string maximalValue;
        private bool resetSeqIdIndicator = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceItem"/> class.
        /// </summary>
        /// <param name="seqno">The seqno.</param>
        public SequenceItem(byte seqno)
        {
            this.seqno = seqno;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceItem"/> class.
        /// </summary>
        /// <param name="seqno">The seqno.</param>
        /// <param name="name">The name.</param>
        /// <param name="variable">The variable.</param>
        /// <param name="length">The length.</param>
        public SequenceItem(byte seqno, string name, string variable, int length)
            : this(seqno)
        {
            this.name = name;
            this.variable = variable;
            this.length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceItem"/> class.
        /// </summary>
        /// <param name="seqno">The seqno.</param>
        /// <param name="name">The name.</param>
        /// <param name="variable">The variable.</param>
        /// <param name="length">The length.</param>
        /// <param name="maximalValue">The maximal value.</param>
        public SequenceItem(byte seqno, string name, string variable, int length, string maximalValue)
            : this(seqno, name, variable, length)
        {
            this.maximalValue = maximalValue;
        }

        #region Members

        /// <summary>
        /// Gets the SEQNO.
        /// </summary>
        /// <value>The SEQNO.</value>
        public byte SEQNO
        {
            get { return seqno; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the variable.
        /// </summary>
        /// <value>The variable.</value>
        public string Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        /// <summary>
        /// Gets or sets the increment.
        /// </summary>
        /// <value>The increment.</value>
        public int Increment
        {
            get { return increment; }
            set { increment = value; }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Gets or sets the pad char.
        /// </summary>
        /// <value>The pad char.</value>
        public Char PadChar
        {
            get { return padChar; }
            set { padChar = value; }
        }

        /// <summary>
        /// Gets or sets the maximal value.
        /// </summary>
        /// <value>The maximal value.</value>
        public string MaximalValue
        {
            get { return maximalValue; }
            set { maximalValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [reset seq id indicator].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [reset seq id indicator]; otherwise, <c>false</c>.
        /// </value>
        public bool ResetSeqIdIndicator
        {
            get { return resetSeqIdIndicator; }
            set { resetSeqIdIndicator = value; }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return name;
        }
    }
}
