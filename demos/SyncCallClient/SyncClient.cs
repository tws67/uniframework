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
        ManualResetEvent handle = null;
        object SyncObj = new object();
        Int32 result = -1;

        public Int32 Invoke(Int32 a, Int32 b)
        {
            lock (SyncObj)
            {
                result = -1;
                byte[] data = serializer.Serialize<Int32[]>(new int[] { a, b });
                Send(data);
            }
            handle = new ManualResetEvent(false);
            handle.WaitOne();
            return result;
        }

        protected override void OnReceivedData(DataBlock dataBlock)
        {
            base.OnReceivedData(dataBlock);
            lock (SyncObj) {
                result = serializer.Deserialize<Int32>(dataBlock.ToArray());
                handle.Set();
                handle = null;
            }
        }
    }
}
