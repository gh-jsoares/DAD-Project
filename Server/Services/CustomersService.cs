using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    public class CustomersService : Customer.CustomerBase
    {
        private readonly ILogger<CustomersService> logger;

        public CustomersService(ILogger<CustomersService> logger)
        {
            this.logger = logger;
        }

        public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModel request, ServerCallContext context)
        {
            CustomerModel output = new CustomerModel();
            switch(request.UserId)
            {
                case 1:
                    output.FirstName = "Roxie";
                    output.LastName = "Jefferson";
                    break;
                case 2:
                    output.FirstName = "Steven";
                    output.LastName = "Jefferson";
                    break;
                case 3:
                    output.FirstName = "Joseph";
                    output.LastName = "Jefferson";
                    break;
                default:
                    output.FirstName = "John";
                    output.LastName = "Doe";
                    break;
            }

            return Task.FromResult(output);
        }

        public override async Task GetNewCustomers(NewCustomerRequest request, IServerStreamWriter<CustomerModel> responseStream, ServerCallContext context)
        {
            List<CustomerModel> customers = new List<CustomerModel>
            {
                new CustomerModel
                {
                    UserId = 4,
                    FirstName = "Luke",
                    LastName = "Smith",
                    Age =  32,
                    IsAlive = true,
                    EmailAddress = "luke.smith@ls.com"
                },
                new CustomerModel
                {
                    UserId = 5,
                    FirstName = "Susan",
                    LastName = "Smith",
                    Age =  34,
                    IsAlive = true,
                    EmailAddress = "susan.smith@ls.com"
                },
                new CustomerModel
                {
                    UserId = 6,
                    FirstName = "Jason",
                    LastName = "Davids",
                    Age =  23,
                    IsAlive = true,
                    EmailAddress = "jason.davids@jd.com"
                },
                new CustomerModel
                {
                    UserId = 7,
                    FirstName = "Thomas",
                    LastName = "Barry",
                    Age =  47,
                    IsAlive = true,
                    EmailAddress = "thomas.barry@tb.com"
                }
            };

            foreach (CustomerModel customer in customers)
            {
                await Task.Delay(1000);
                await responseStream.WriteAsync(customer);
            }
        }
    }
}
