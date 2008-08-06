// ***************************************************************
//  version:  1.0   date: 11/27/2007
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  (C)2007 Midapex All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Uniframework
{
    /// <summary>
    /// 泛型事件参数（单个参数）
    /// </summary>
    /// <typeparam name="ParamType">事件参数类型</typeparam>
    public class TEventArgs<ParamType> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TEventArgs&lt;ParamType&gt;"/> class.
        /// </summary>
        /// <param name="param">The param.</param>
        public TEventArgs(ParamType param)
        {
            this.param = param;
        }

        ParamType param;

        /// <summary>
        /// Gets the param.
        /// </summary>
        /// <value>The param.</value>
        public ParamType Param
        {
            get { return param; }
        }
    }

    /// <summary>
    ///  泛型事件参数（两个参数）
    /// </summary>
    /// <typeparam name="Param1Type">事件参数１类型</typeparam>
    /// <typeparam name="Param2Type">事件参数２类型</typeparam>
    public class TEventArgs<Param1Type, Param2Type>:EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TEventArgs&lt;Param1Type, Param2Type&gt;"/> class.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <param name="param2">The param2.</param>
        public TEventArgs(Param1Type param1, Param2Type param2)
        {
            this.param1 = param1;
            this.param2 = param2;
        }

        Param1Type param1;

        /// <summary>
        /// Gets the param1.
        /// </summary>
        /// <value>The param1.</value>
        public Param1Type Param1
        {
            get { return param1; }
        }

        Param2Type param2;

        /// <summary>
        /// Gets the param2.
        /// </summary>
        /// <value>The param2.</value>
        public Param2Type Param2
        {
            get { return param2; }
        }
    }

    /// <summary>
    ///  泛型事件参数（三个参数）
    /// </summary>
    /// <typeparam name="Param1Type">事件参数１类型</typeparam>
    /// <typeparam name="Param2Type">事件参数２类型</typeparam>
    /// <typeparam name="Param3Type">事件参数 3类型</typeparam>
    public class TEventArgs<Param1Type, Param2Type, Param3Type> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TEventArgs&lt;Param1Type, Param2Type, Param3Type&gt;"/> class.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <param name="param2">The param2.</param>
        /// <param name="param3">The param3.</param>
        public TEventArgs(Param1Type param1, Param2Type param2,Param3Type param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
        }

        Param1Type param1;
        Param2Type param2;
        Param3Type param3;

        /// <summary>
        /// Gets the param1.
        /// </summary>
        /// <value>The param1.</value>
        public Param1Type Param1
        {
            get { return param1; }
        }
        /// <summary>
        /// Gets the param2.
        /// </summary>
        /// <value>The param2.</value>
        public Param2Type Param2
        {
            get { return param2; }
        }

        /// <summary>
        /// Gets or sets the param3.
        /// </summary>
        /// <value>The param3.</value>
        public Param3Type Param3
        {
            get { return param3; }
            set { param3 = value; }
        }
    }
}
