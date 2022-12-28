using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace qRPC.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("qRPC!");

            Console.ReadKey();
            var doStuff = new DoStuff();

            var server = new qRPC.Server.QrpcServer<IDoStuff>(doStuff, 5713, Encoding.UTF8);

            var client = new qRPC.Client.QrpcClient<IDoStuff>(5713, "127.0.0.1", Encoding.UTF8);
            var client2 = new qRPC.Client.QrpcClient<IDoStuff>(5713, "127.0.0.1", Encoding.UTF8);
            
            CancellationToken token = new CancellationToken(false);

            //var task = server.Listen(token);

            try
            {
                DateTime start = DateTime.UtcNow;

                var now1 = Task.Run(() => client.Remote.GetNow());

                var res = Task.Run(() => client.Remote.HiWorld("Dan"));

                var b = Task.Run(() => client2.Remote.HiWorld("Felix"));

                var c = Task.Run(() => client.Remote.HiWorld("Papá"));

                var d = Task.Run(() => client.Remote.HiWorld("Son"));

                var x = Task.Run(() => client.Remote.TryAGuid(Guid.NewGuid()));

                Console.WriteLine(res.Result);
                Console.WriteLine(b.Result);
                Console.WriteLine(c.Result);

                Console.WriteLine(d.Result);

                Console.WriteLine(x.Result);

                Console.WriteLine($"{(DateTime.UtcNow - start).TotalSeconds}s");

                Console.WriteLine(client.Remote.GetAGuid());
                Console.WriteLine($"{(DateTime.UtcNow - start).TotalSeconds}s");

                var z = client.Remote.Addition(4.3, 6.2);
                Console.WriteLine($"4.3 + 6.2 = ${z}");
                Console.WriteLine($"{(DateTime.UtcNow - start).TotalSeconds}s");

                var ticks = client.Remote.TryADatetime(DateTime.UtcNow);
                Console.WriteLine($"Number of ticks: {ticks}");
                Console.WriteLine($"{(DateTime.UtcNow - start).TotalSeconds}s");

                var e = client.Remote.WhatIsMyNumber("Gem", 478);
                Console.WriteLine(e);
                Console.WriteLine($"{(DateTime.UtcNow - start).TotalSeconds}s");


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
