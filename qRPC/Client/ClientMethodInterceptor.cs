using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace qRPC.Client
{
    /// <summary>
    /// Interceptor class which passes back control to the TCP client via a delegate.
    /// </summary>
    internal class ClientMethodInterceptor : IInterceptor
    {
        private readonly CallDelegate _delegate;

        public ClientMethodInterceptor(CallDelegate @delegate)
        {
            this._delegate = @delegate;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = this._delegate.DynamicInvoke(invocation); 
        }
    }
}
