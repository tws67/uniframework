using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniframework;
using Uniframework.Net;

namespace SyncCallServer
{
    public class SyncServer : TcpServerBase<TcpSession>
    {
        Serializer serializer = new Serializer();

        protected override void OnReceivedData(TcpSession session, DataBlock dataBlock)
        {
            base.OnReceivedData(session, dataBlock);

            Int32[] arrays = serializer.Deserialize<Int32[]>(dataBlock.ToArray());
            if (arrays.Length == 2) {
                byte[] results = serializer.Serialize<Int32>(arrays[0] + arrays[1]);
                Send(session, results);
            }
        }
    }
}
