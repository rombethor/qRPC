using System;
using System.Collections.Generic;
using System.Text;

namespace qRPC.Transport
{
    public class QrpcRequest
    {
        public string MethodName { get; set; }
        public string[] Arguments { get; set; }
    }
}
 