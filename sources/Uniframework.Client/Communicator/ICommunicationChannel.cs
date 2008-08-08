using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    public interface ICommunicationChannel
    {
        byte[] Invoke(byte[] data);
    }
}
