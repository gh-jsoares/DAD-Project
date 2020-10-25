using ProcessCreationService.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;

namespace ProcessCreationService
{

    

    class Program
    {
        private static CommandExecutor commands;

        private const int Port = 10000;

        static async Task Main(string[] args)
        {

            Server server = new Server
            {
                Services = { CommandListener.BindService(new ProcessCreationService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("ProcessCreationService listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }

        /*private static async Task DoGrpcStuff()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");

            //HelloRequest input = new HelloRequest { Name = "Rox" };
            //Greeter.GreeterClient client = new Greeter.GreeterClient(channel);
            //HelloReply reply = await client.SayHelloAsync(input);
            //Console.WriteLine(reply.Message);

            Customer.CustomerClient client = new Customer.CustomerClient(channel);

            CustomerLookupModel input = new CustomerLookupModel { UserId = 1 };
            CustomerModel reply = await client.GetCustomerInfoAsync(input);
            Console.WriteLine("Received customer: {0} {1}", reply.FirstName, reply.LastName);

            using (AsyncServerStreamingCall<CustomerModel> call = client.GetNewCustomers(new NewCustomerRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    CustomerModel customer = call.ResponseStream.Current;
                    Console.WriteLine("New customer: {0} {1}", customer.FirstName, customer.LastName);
                }
            }
        }*/
    }
}
