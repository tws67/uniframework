using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Uniframework;
using Uniframework.Net;

namespace SyncCallClient
{
    public class SyncClient : TcpClientBase<TcpSession>
    {
        Serializer serializer = new Serializer();
        WaitHandle waitHandle = new WaitHandle();
        Int32 result = -1;

        public Int32 Invoke(Int32 a, Int32 b)
        {
            result = -1;
            byte[] data = serializer.Serialize<Int32[]>(new int[] {a, b});
            Send(data);
            waitHandle.WaitOne();
            return result;
        }

        protected override void OnReceivedData(DataBlock dataBlock)
        {
            base.OnReceivedData(dataBlock);
            result = serializer.Deserialize<Int32>(dataBlock.ToArray());
        }
    }
}
