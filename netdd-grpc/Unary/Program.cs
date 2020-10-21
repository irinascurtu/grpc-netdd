using Grpc.Core;
using netdd_grpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unary
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:5000", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);
            var cts = new CancellationTokenSource();

            HelloRequest request = new HelloRequest() { Name = "netdd" };
            Console.WriteLine($"sending: {request.Name}");
            var reply = client.SayHello(request, options: new CallOptions() { });

            Console.WriteLine(reply.Message);
        }
    }
}
