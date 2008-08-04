using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 序号注册表
    /// </summary>
    [Serializable]
    public class Sequence
    {
        private Guid sequenceId = CombinUtility.NewComb();
        private string name;
        private bool enable = true;
        private int poolSize = 30;
        private bool circleIndicator = false;
        private ISequenceGenerater generater = null;
        private string maxValue;
        private string curValue;
        private int fillValve = 5;
        private string comment;
        private Dictionary<byte, SequenceItem> items = new Dictionary<byte, SequenceItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Sequence(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="generater">The generater.</param>
        public Sequence(string name, ISequenceGenerater generater)
            : this(name)
        {
            this.generater = generater;
        }

        #region Members

        /// <summary>
        /// Gets or sets the sequence id.
        /// </summary>
        /// <value>The sequence id.</value>
        public Guid SequenceId
        {
            get { return sequenceId; }
            set { sequenceId = value; }
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
        /// Gets or sets a value indicating whether this <see cref="Sequence"/> is enable.
        /// </summary>
        /// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [circle indicator].
        /// </summary>
        /// <value><c>true</c> if [circle indicator]; otherwise, <c>false</c>.</value>
        public bool CircleIndicator
        {
            get { return circleIndicator; }
            set { circleIndicator = value; }
        }

        /// <summary>
        /// Gets the generater.
        /// </summary>
        /// <value>The generater.</value>
        public ISequenceGenerater Generater
        {
            get
            {
                if (generater == null)
                    generater = new DefaultSequenceGenerater(this);
                return generater;
            }
        }

        /// <summary>
        /// Gets or sets the size of the pool.
        /// </summary>
        /// <value>The size of the pool.</value>
        public int PoolSize
        {
            get { return poolSize; }
            set { poolSize = value; }
        }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        public string MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        /// <summary>
        /// Gets or sets the cur value.
        /// </summary>
        /// <value>The cur value.</value>
        public string CurValue
        {
            get { return curValue; }
            internal set { curValue = value; }
        }

        /// <summary>
        /// Gets or sets the fill valve.
        /// </summary>
        /// <value>The fill valve.</value>
        public int FillValve
        {
            get { return fillValve; }
            set { fillValve = value; }
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public Dictionary<byte, SequenceItem> Items
        {
            get { return items; }
        }

        #endregion

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(SequenceItem item)
        {
            items[item.SEQNO] = item;
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(SequenceItem item)
        {
            if (items.ContainsKey(item.SEQNO))
                items.Remove(item.SEQNO);
        }
    }
}
