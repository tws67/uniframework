using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultSequenceGenerater : ISequenceGenerater
    {
        private Sequence sequence;
        private Dictionary<string, string> variables = new Dictionary<string, string>();
        private object syncObj = new object();
        private int comparelength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSequenceGenerater"/> class.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public DefaultSequenceGenerater(Sequence sequence)
        {
            this.sequence = sequence;
        }

        #region ISequenceGenerater Members

        /// <summary>
        /// 生成序列的下一个可用序号
        /// </summary>
        /// <returns></returns>
        public string GenerateNextID()
        {
            lock (syncObj)
            {
                string curSeqId = sequence.CurValue;
                if (curSeqId == sequence.MaxValue)
                    throw new UniframeworkException("序列达到最大值不能再生成新的序号请调整序号设置。");

                string tmpSeqId = string.Empty;
                int len = 0, seq = 0;
                for (byte i = 0; i < sequence.Items.Count; i++)
                {
                    if (i != sequence.Items.Count - 1)
                    {
                        tmpSeqId += Parse(sequence.Items[i]);
                        len += sequence.Items[i].Length;
                    }
                    else
                    {
                        SequenceItem resetItem = GetResetSeqIdItem();
                        if (resetItem != null)
                        {
                            if (sequence.CurValue.Substring(0, comparelength) != tmpSeqId.Substring(0, comparelength))
                                seq = 0;
                            else
                                seq = int.Parse(sequence.CurValue.Substring(comparelength, sequence.CurValue.Length - comparelength));
                            tmpSeqId += (++seq).ToString().PadLeft(sequence.Items[i].Length, sequence.Items[i].PadChar);
                        }
                    }
                }
                sequence.CurValue = tmpSeqId;
                return sequence.CurValue;
            }
        }
        
        #endregion

        #region Assistants function

        /// <summary>
        /// 分析序列号的每一段组成
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string Parse(SequenceItem item)
        {
            string tmpStr = "";
            if (item.Variable.IndexOf("${") != -1 && item.Variable.IndexOf("}") != -1)
                switch (item.Variable)
                {
                    case "${Year}":
                        tmpStr = DateTime.Now.ToString("yyyy");
                        break;

                    case "${Month}":
                        tmpStr = DateTime.Now.ToString("MM");
                        break;

                    case "${Date}":
                        tmpStr = DateTime.Now.ToString("yyyyMMdd");
                        break;

                    case "${DateTime}":
                        tmpStr = DateTime.Now.ToString("yyyyMMddhhmmss");
                        break;
                }
            else
                tmpStr = item.Variable;
            return tmpStr.Substring(0, item.Length);
        }

        private SequenceItem GetResetSeqIdItem()
        {
            comparelength = 0;
            foreach (SequenceItem item in sequence.Items.Values)
            {
                comparelength += item.Length;
                if (item.ResetSeqIdIndicator)
                    return item;
            }
            return null;
        }

        #endregion
    }
}
