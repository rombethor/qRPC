using System;
using System.Text;
using System.Threading;

namespace qRPC.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("qRPC!");

            Console.ReadKey();
            var doStuff = new DoStuff();

            var server = new qRPC.Server.QrpcServer<IDoStuff>(doStuff, "127.0.0.1", 5000, Encoding.UTF8);

            var client = new qRPC.Client.QrpcClient<IDoStuff>(5000, "127.0.0.1", Encoding.UTF8);

            CancellationToken token = new CancellationToken(false);

            //var task = server.Listen(token);

            try
            {
                var res = client.Remote.HiWorld("Dan");
                Console.WriteLine(res);

                var b = client.Remote.HiWorld("Felix");
                Console.WriteLine(b);

                var c = client.Remote.HiWorld("Dan");
                Console.WriteLine(c);

                var d = client.Remote.HiWorld("Felix");
                Console.WriteLine(d);

                var x = client.Remote.TryAGuid(Guid.NewGuid());
                Console.WriteLine(x);

                Console.WriteLine(client.Remote.GetAGuid());

                var z = client.Remote.Addition(4.3, 6.2);
                Console.WriteLine($"4.3 + 6.2 = ${z}");

                var ticks = client.Remote.TryADatetime(DateTime.UtcNow);
                Console.WriteLine($"Number of ticks: {ticks}");

                var e = client.Remote.WhatIsMyNumber("Gem", 478);
                Console.WriteLine(e);


                client.Remote.NotImplemented();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.ReadKey();
            
        }
    }
}
