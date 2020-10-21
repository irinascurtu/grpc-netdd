using Grpc.Core;
using System;
using netdd_grpc;
using System.Threading;
using System.Threading.Tasks;

namespace Client
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

           // using var streamingCall = client.ServerStream(new HelloRequest(), cancellationToken: cts.Token);
            using var streamingCall = client.ServerStream(new HelloRequest(), 
             deadline: DateTime.UtcNow.AddMilliseconds(1), cancellationToken: cts.Token);

            try
            {
                await foreach (HelloReply reply in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {

                    Console.WriteLine($"{reply.Message}");
                }

                var trailers = streamingCall.GetTrailers();
                var myValue = trailers.GetValue("my-fake-header");
                Console.WriteLine($"found some trailer trailer values:{myValue}");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Greeting timeout.");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }

    }
}
