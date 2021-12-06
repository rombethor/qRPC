using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace qRPC.Client
{
    internal delegate object CallDelegate(IInvocation invocation);
}
