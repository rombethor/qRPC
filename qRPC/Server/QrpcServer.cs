using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using qRPC.Transport;

namespace qRPC.Server
{

    public class QrpcServer<T> : IDisposable
    {
        T _target;

        public delegate T TargetDelegate();

        Task listening;
        TcpListener tcp;
        Encoding _encoding;

        CancellationTokenSource cts;

        public QrpcServer(T target, int port, Encoding encoding) 
        {
            Setup(target, port, encoding);
        }

        public QrpcServer(TargetDelegate targetDelegate, int port, Encoding encoding) 
        {
            Setup(targetDelegate.Invoke(), port, encoding);
        }

        void Setup(T target, int port, Encoding encoding)
        {
            _target = target;
            _encoding = encoding;
            tcp = new TcpListener(IPAddress.Any, port);
            tcp.Start();
            cts = new CancellationTokenSource();
            listening = Listen(cts.Token);
        }

        //Proxy not required for server.  Just use Reflection.

        public async Task Listen(CancellationToken cancellationToken)
        {
            //Listen for connections
            while(!cancellationToken.IsCancellationRequested)
            {
                var client = await tcp.AcceptTcpClientAsync(cancellationToken);
                ThreadPool.QueueUserWorkItem(ProcessRequest, client);
            }
            tcp.Stop();
        }
        
        private void ProcessRequest(object objClient)
        {
            TcpClient client = (TcpClient)objClient;
            using NetworkStream stream = client.GetStream();
            try
            {
                int attempts = 0;
                while (client.Connected && client.Available == 0 && attempts < 10)
                {
                    Thread.Sleep(100);
                    attempts++;
                }
                if (client.Connected)
                {
                    var message = stream.ReadObjectFromStream<QrpcRequest>(_encoding);

                    try
                    {
                        object response = ExecuteTask(message.MethodName, message.Arguments);
                        stream.WriteObjectToStream(response, _encoding);
                        stream.Flush();
                    }
                    catch(Exception ex)
                    {
                        throw new MethodProcessingException(ex, message.MethodName, message.Arguments);
                    }

                }
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public object ExecuteTask(string methodName, params object[] args)
        {
            var methodInfo = typeof(T).GetMethod(methodName);

            var parameters = methodInfo.GetParameters().OrderBy(p => p.Position).ToArray();

            if (args.Length < parameters.Where(p => !p.IsOptional).Count())
                throw new ArgumentException("The number of arguments provided doesn't fulfill the method requirements");

            List<object?> passArgs = new();

            for(int i = 0; i < parameters.Count(); i++)
            {
                Type paramType = parameters[i].ParameterType;
                passArgs.Add(JsonSerializer.Deserialize(args[i].ToString(), paramType));
            }

            var method = typeof(T).GetMethod(methodName);
            var ret = method.Invoke(_target, passArgs.ToArray());


            return ret; // method.Invoke(_target, args);
        }

        public void Dispose()
        {
            cts.Cancel();
        }
    }
}
