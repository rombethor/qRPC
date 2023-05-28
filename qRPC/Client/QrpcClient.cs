using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using qRPC.Transport;

namespace qRPC.Client
{
    public class QrpcClient<IType>
    {
        readonly int _port;
        readonly string _hostname;
        readonly Encoding _encoding;
        public QrpcClient(int port, string hostname, Encoding encoding)
        {
            _port = port;
            _hostname = hostname;
            _encoding = encoding;

            _remote = GenerateClientProxy();
        }

        /// <summary>
        /// The proxy
        /// </summary>
        readonly IType _remote;

        /// <summary>
        /// Use this implementation of the interface to make the remote calls.
        /// </summary>
        public IType Remote { get => _remote; }


        protected IType GenerateClientProxy()
        {
            var clientInterceptor = new ClientMethodInterceptor(ExecuteRPC);
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IType), clientInterceptor);
            return (IType)proxy;
        }

        private object ExecuteRPC(IInvocation invocation)
        {
            //send data over TCP
            using (TcpClient tcp = new TcpClient(_hostname, _port) { ReceiveTimeout = 30000 })
            {
                var stream = tcp.GetStream();
                var message = new Transport.QrpcRequest()
                {
                    MethodName = invocation.Method.Name,
                    Arguments = invocation.Arguments
                };

                //stream.WriteObjectToStream(message, _encoding);

                stream.WriteObjectToStream(message, _encoding);
                    
                    while (tcp.Available == 0 && tcp.Connected)
                    { }
                    object obj = null;
                    if(tcp.Connected)
                    {
                        
                        obj = stream.ReadObjectFromStream(_encoding, invocation.Method.ReturnType);
                    }

                    tcp.Close();
                    return obj;
            }

        }

    }
}
