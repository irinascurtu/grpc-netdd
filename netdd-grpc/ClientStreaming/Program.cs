using Grpc.Core;
using netdd_grpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientStreaming
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:5000", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);
            //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            ///Server Stream
            var cts = new CancellationTokenSource();

            try
            {
                using var call = client.ClientStream();

                for (var i = 0; i < 100000; i++)
                {

                    await call.RequestStream.WriteAsync(new HelloRequest { Name = i.ToString() });
                }

                await call.RequestStream.CompleteAsync();
                HelloReply response = await call;

                Console.WriteLine($"Received: {response.Message}");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }
    }
}
