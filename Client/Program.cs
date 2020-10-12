using Client.Commands;
using Grpc.Core;
using Grpc.Net.Client;
using Server;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static CommandExecutor commands;

        static async Task Main(string[] args)
        {
            //await DoGrpcStuff();

            commands = new CommandExecutor();

            // FAIL CASES
            commands.Run("wait    ");
            commands.Run("wait    10   32  ");

            // SUCCESS CASES
            commands.Run("wait    10");
            commands.Run("wait 10");

            Console.ReadLine();
        }

        private static async Task DoGrpcStuff()
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
        }
    }
}
