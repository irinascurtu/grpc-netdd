using Grpc.Core;
using netdd_grpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BiDirectional
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:5000", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);
            var cts = new CancellationTokenSource();
            try
            {


                using (var call = client.BiDirectional())
                {
                    var responseReaderTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            HelloReply message = call.ResponseStream.Current;
                            Console.WriteLine("Received " + message.Message);
                        }
                    });

                    var request = new HelloRequest();
                    for (int i = 0; i < 10; i++)
                    {
                        request.Name = i.ToString();
                        Console.WriteLine("Sending " + request.Name);
                        await call.RequestStream.WriteAsync(request);
                    }
                    await call.RequestStream.CompleteAsync();
                    await responseReaderTask;
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }
    }
}
